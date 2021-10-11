using MelanomaClassification.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MelanomaClassification.Services
{
    public class DatabaseService
    {
        private static SQLiteAsyncConnection DbConn;
        //FileSystem is not supported in the 
        private static string ConnString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "localDb.db");

        public static async Task<bool> LoadData()
        {
            var listImages = await GetUserImageDataAsync();
            var listPredicts = await GetUserPredictionDataAsync();

            List<ModelPredictionWrapper> wrappers = ImageUtilityService.CreatePredictionWrapper(listImages, listPredicts);
            foreach (var wrapper in wrappers)
            {
                ModelPhotoGallery.NewPredictions.Add(wrapper);
            }
            return true;
        }
        
        private static string TableName = "predictionTbl";
        private static List<SQL_ModelPrediction> DeleteMemoi = new List<SQL_ModelPrediction>();
        private static List<SQL_ModelPrediction> UpdateMemoi = new List<SQL_ModelPrediction>();
        public static volatile bool finishedInit = false;
        
        public static void WaitUntilFinished()
        {
            while (!finishedInit)
            {
                System.Threading.Thread.Sleep(50);
            }
            finishedInit = false;
        }
        public static async Task<bool> Init()
        {
            if (DbConn == null)
            {
                DbConn = new SQLiteAsyncConnection(ConnString);
                await CheckIfExisted();
            }
            finishedInit = true;
            return finishedInit;
        }

        public static async Task<bool> CheckIfExisted()
        {
            //string name = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            int imageTblExists = 0;
            int predictTblExists = 0;
            int numTabls = 0;
            try
            {
                 imageTblExists= await DbConn.ExecuteScalarAsync<int>
                    ("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='imageTbl'");
                predictTblExists = await DbConn.ExecuteScalarAsync<int>
                   ("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='predictionTbl'");

                if (imageTblExists != 1)
                {
                     await DbConn.CreateTableAsync<SQL_ModelImage>();
                    imageTblExists = await DbConn.ExecuteScalarAsync<int>
                    ("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='imageTbl'");
                }
                 if (predictTblExists != 1)
                {
                    await DbConn.CreateTableAsync<SQL_ModelPrediction>();

                    predictTblExists = await DbConn.ExecuteScalarAsync<int>
                    ("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='predictionTbl'");

                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            if (imageTblExists + predictTblExists < 2)
            {
                numTabls = await DbConn.ExecuteScalarAsync<int>
                    ("SELECT COUNT(*) FROM sqlite_master WHERE type='table'");
                throw new Exception("table not successfully created");

            }

            return numTabls == 2;

        }

       
        public static void DeleteByIdAsync(string id)
        {
         
            Task<int> t = DbConn.Table<SQL_ModelImage>().DeleteAsync(item => item.ParentId == id);
            Task<int> t1 = DbConn.Table<SQL_ModelPrediction>().DeleteAsync(item => item.ParentId == id);
            
            Task.WaitAll(t, t1);
            
        }
        public static async Task PutAsync(ModelPredictionWrapper predictWrapper)
        {
            int index = await GetNumberImagesCurrentUser() + 1;
            try
            {
                var image = new SQL_ModelImage
                {
                    Date = DateTime.Now.ToString("mm:HH dd/MM/yyyy"),
                    Id = index,
                    ImageData = predictWrapper.ImageData
                };
                //already in the percentage form
                var item1 = new SQL_ModelPrediction
                {
                    Prob = predictWrapper.Predictions[0].Probability.ToString(),
                    Id = index,
                    Tag = predictWrapper.Predictions[0].TagName
                };

                var item2 = new SQL_ModelPrediction
                {
                    Prob = predictWrapper.Predictions[1].Probability.ToString(),
                    Id = index,
                    Tag = predictWrapper.Predictions[1].TagName
                };

                var item3 = new SQL_ModelPrediction
                {
                    Prob = predictWrapper.Predictions[2].Probability.ToString(),
                    Id = index,
                    Tag = predictWrapper.Predictions[2].TagName
                };
                List<SQL_ModelPrediction> list = new List<SQL_ModelPrediction>();
                list.Add(item2); list.Add(item1); list.Add(item3);
                //can't put them concurrently (a deadlock may occur)
                await PutAllAsync(list);
                await PutAsync(image);
                
            }
            catch (InvalidCastException)
            {
                Console.WriteLine("cast exceptionn");
            }
           
            
        }

        public static async Task<int> DropAllTables()
        {
            int n = await DbConn.DropTableAsync<SQL_ModelPrediction>();
            int n1 = await DbConn.DropTableAsync<SQL_ModelImage>();
            await DbConn.CloseAsync();
            DbConn = null;
            return n + n1;
        }
        public static async Task<int> GetTotalNumber()
        {
            return await DbConn.Table<SQL_ModelPrediction>().CountAsync();
        }

        public static async Task PutAsync(SQL_ModelPrediction prediction)
        {
            
            await DbConn.InsertAsync(prediction);

        }

        public static async Task<bool> PutAllAsync(List<SQL_ModelPrediction> rawData)
        {
            await DbConn.InsertAllAsync(rawData);
            return true;
        }

        public static async Task<bool> DeleteCurrentUser()
        {
            
            Task<int> deletedPredicts = DbConn.ExecuteAsync("DELETE FROM predictionTbl WHERE Username='?'", new string[] { ModelAccountPage.Username });
            Task<int> deletedImages = DbConn.ExecuteAsync("DELETE FROM imageTbl WHERE Username='?'", new string[] { ModelAccountPage.Username });
            Task.WaitAll(deletedImages, deletedPredicts);

            return deletedImages.Result + deletedPredicts.Result == 2;
        }

        public static async Task PutAsync(SQL_ModelImage image)
        {
            await DbConn.InsertAsync(image);
        }



        public static async void EraseAll()
        {
            int num = await DbConn.DropTableAsync<SQL_ModelPrediction>();
            var res = await DbConn.CreateTableAsync<SQL_ModelPrediction>();
        }


        public static async Task<bool> DeleteData(int Id)
        {
            int hasDeleted =await DbConn.Table<SQL_ModelPrediction>().DeleteAsync(model => model.Id == Id);
            int ct = await DbConn.Table<SQL_ModelPrediction>().CountAsync(model => model.Username == ModelAccountPage.Username);
            Debug.WriteLine("Ct = " + ct);

            return hasDeleted == 1;
        }
        public static async Task<List<SQL_ModelPrediction>> GetUserPredictionDataAsync()
        {

            var table = DbConn.Table<SQL_ModelPrediction>();
            var items = table.Where(item => item.Username == ModelAccountPage.Username);
            return await items.ToListAsync();

        }
        public static async Task<List<SQL_ModelImage>> GetUserImageDataAsync()
        {

            var table = DbConn.Table<SQL_ModelImage>();
            var items = table.Where(item => item.Username == ModelAccountPage.Username);
            var nItem = await items.CountAsync();
            return await items.ToListAsync();

        }

        public static async Task<int> GetNumberImagesCurrentUser()
        {
            var number = await DbConn.Table<SQL_ModelImage>().CountAsync(item => item.Username == ModelAccountPage.Username);
           
            return number;
        }
       public static async Task<int> GetNumberPredictionsCurrentUser()
        {
            var number = await DbConn.Table<SQL_ModelPrediction>().CountAsync(item => item.Username == ModelAccountPage.Username);

            return number;

        }

        

    }
}
