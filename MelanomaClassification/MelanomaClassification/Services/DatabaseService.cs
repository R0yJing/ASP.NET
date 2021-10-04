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
        private static string TableName = "predictionTbl";
        private static List<SQL_ModelPrediction> DeleteMemoi = new List<SQL_ModelPrediction>();
        private static List<SQL_ModelPrediction> UpdateMemoi = new List<SQL_ModelPrediction>();

        public static async void Init()
        {
            if (DbConn == null)
            {
                DbConn = new SQLiteAsyncConnection(ConnString);
                await CheckIfExisted();
            }
        }

        public static async Task<bool> CheckIfExisted()
        {
            //string name = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            int numTables =0;
            try
            {
                numTables = await DbConn.ExecuteScalarAsync<int>
                    ("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='predictionTbl'");
            }catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            if (numTables == 0)
            {
                await DbConn.CreateTableAsync<SQL_ModelPrediction>();
                numTables = await DbConn.ExecuteScalarAsync<int>
                    ("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='predictionTbl'");
            }
            return numTables == 1;

        }

        public static async void PutAsync(ModelPredictionWrapper prediction)
        {
            
            var newInsertion = new SQL_ModelPrediction
            {
                ImageData = prediction.ImageData,
                Username = ModelAccountPage.Username,
                Prob = prediction.Prob,
                Date = prediction.Date,
                Tag = prediction.Tag
                
            };
            await DbConn.InsertAsync(newInsertion);

        }

        public static async Task<bool> PutAllAsync(List<SQL_ModelPrediction> rawData)
        {
            await DbConn.InsertAllAsync(rawData);
            return true;
        }

        public static void PutAll(List<ModelPredictionWrapper> predictions)
        {
            predictions.ForEach(prediction => PutAsync(prediction));

        }
        public static async Task<bool> DeleteCurrentUser()
        {
            int hasDeleted = await DbConn.DeleteAsync<SQL_ModelPrediction>(ModelAccountPage.Username);
            return hasDeleted == 1;
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
        public static async Task<List<SQL_ModelPrediction>> GetAllAsync()
        {

            var table = DbConn.Table<SQL_ModelPrediction>();
            var items = table.Where(item => item.Username == ModelAccountPage.Username);
            return await items.ToListAsync();

        }

        public static async Task<int> GetNumberItemsCurrentUser()
        {
            /*            return await DbConn.Table<SQL_ModelPrediction>().CountAsync(item => item.Username == ModelAccountPage.Username);
            */
            var number = await DbConn.Table<SQL_ModelPrediction>().CountAsync(item => item.Username == ModelAccountPage.Username);

            return number;
        }
    }
}
