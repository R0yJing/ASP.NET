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
        private static string ConnString = Path.Combine(FileSystem.AppDataDirectory, "localDb.db");
        public static void Init()
        {
            if (DbConn == null)
            {
                DbConn = new SQLiteAsyncConnection(ConnString);
                CheckIfExisted();
            }
        }

        private static async void CheckIfExisted()
        {

            int numTables = await DbConn.ExecuteScalarAsync<int>
                ("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='predictionTbl'");
            if (numTables == 0)
            {
                await DbConn.CreateTableAsync<SQL_ModelPrediction>();

            }
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
        public static async Task<bool> PutAll(List<SQL_ModelPrediction> predictions)
        {
            return await DbConn.InsertAllAsync(predictions) == 1;
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

        public static async Task<bool> DeleteData(int Id)
        {
            int hasDeleted =await DbConn.Table<SQL_ModelPrediction>().DeleteAsync(model => model.Id == Id);
            int ct = await DbConn.Table<SQL_ModelPrediction>().CountAsync(model => model.Username == ModelAccountPage.Username);
            Debug.WriteLine("Ct = " + ct);

            return hasDeleted == 1;
        }
        public static async Task<List<SQL_ModelPrediction>> GetAllAsync()
        {
            List<SQL_ModelPrediction> predictions = await DbConn.QueryAsync<SQL_ModelPrediction>("SELECT * FROM predictionTbl WHERE Username = ?", new string[] { ModelAccountPage.Username });
           

            return predictions;
        }
    }
}
