using SQLite;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MelanomaClassification.Models
{
    public class ModelDatabase
{

    /*static SQLiteAsyncConnection Database;

    public static readonly AsyncLazy<ModelDatabase> Instance = new AsyncLazy<ModelDatabase>(async () =>
    {
        var instance = new ModelDatabase();
        CreateTableResult result = await Database.CreateTableAsync<ModelDB_Item>();
        return instance;
    });

    public ModelDatabase()
    {
        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
    }*/

    //...
}

   /* public class AsyncLazy<T>
    {
        readonly Lazy<Task<T>> instance;
        public AsyncLazy(Func<T> factory){
            instance = new Lazy<Task<T>>(() => Task.Run(factory));
        }
        public TaskAwaiter<T> GetAwaiter()
        {
            return instance.Value.GetAwaiter();
        }
    }*/

    /*public Task<List<ModelDB_Item>> GetItemsAsync()
    {
        return Data
    }*/
}
