using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Matrix_UWP.Services {
  public class Database {
    private Database(string name = "MatrixUWP.db") {
      var storagePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, name);
      Debug.WriteLine($"Open Db {storagePath}");
      Connection = new SqliteConnection($"Data Source={storagePath}");
      Connection.Open();
    }
    public SqliteConnection Connection { get; set; }
    static public Database Service = new Database();
  }
}
