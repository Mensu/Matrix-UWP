using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.Model
{
    class SuggestionInput
    {
        public static SQLiteConnection conn;
        public static string dbname = "demo.db";
        public static string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbname);
        private static SuggestionInput instance;
        private SuggestionInput()
        {
            loadDatabase();
        }

        public static SuggestionInput GetInstance()
        {
            if (instance == null) instance = new SuggestionInput();
            return instance;
        }

        public ObservableCollection<string> loadUser(string user)
        {
            var items = new ObservableCollection<string>();
            int i = 0;
            using (var statement = conn.Prepare("SELECT ID FROM USER_NAME"))
            {
                while (statement.Step() == SQLiteResult.ROW && i < 5)
                {
                    var item = (string)statement[0];
                    if (item.Contains(user))
                    {
                        items.Add(item);
                        i++;
                    }
                }
            }
            return items;
        }

        public void addUser(string user)
        {
            var db = conn;
            using (var statement = db.Prepare("INSERT INTO USER_NAME (ID) VALUES(?)"))
            {
                statement.Bind(1, user);
                statement.Step();
            }
        }

        private void loadDatabase()
        {
            conn = new SQLiteConnection("demo.db");
            string sql = @"CREATE TABLE IF NOT EXISTS
                               USER_NAME (ID VARCHAR(30) PRIMARY KEY NOT NULL);";
            using (var statement = conn.Prepare(sql))
            {
                statement.Step();
            }
        }
    }
}