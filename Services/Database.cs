using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Matrix_UWP.Services {
  public class Database {
    private Database(string name = "MatrixUWP.db") {
      var storagePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, name);
      Connection = new SqliteConnection($"Data Source={storagePath};Version=3;");
    }
    public SqliteConnection Connection { get; set; }
    static public Database Service = new Database();
  }
}
