using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.Services {
  public class UsernameSuggest {
    static public UsernameSuggest Service = new UsernameSuggest();
    private UsernameSuggest() {
      string initialSql = @"
        CREATE TABLE IF NOT EXISTS
        USERNAME_SUGGESTION (
          USERNAME VARCHAR(30) PRIMARY KEY NOT NULL
        );
      ";
      SqliteCommand initialCommand = new SqliteCommand(initialSql, Database.Service.Connection);
      initialCommand.ExecuteReader();
    }

    private readonly string retrieveCandidates = @"
      SELECT USERNAME
      FROM USERNAME_SUGGESTION
      WHERE USERNAME LIKE $PATTERN;
    ";

    public async Task<List<string>> RetrieveCandidates(string input) {
      var connection = Database.Service.Connection;
      var command = connection.CreateCommand();
      command.CommandText = retrieveCandidates;
      command.Parameters.AddWithValue("$PATTERN", $"%{input}%");
      var query = await command.ExecuteReaderAsync();
      List<string> candidates = new List<string>();
      while (await query.ReadAsync()) {
        candidates.Add(query.GetString(0));
      }
      return candidates;
    }

    private readonly string addUser = @"
      INSERT OR IGNORE INTO
      USERNAME_SUGGESTION(USERNAME) VALUES($USERNAME);
    ";
    public void Add(string username) {
      var connection = Database.Service.Connection;
      var command = connection.CreateCommand();
      command.CommandText = addUser;
      command.Parameters.AddWithValue("$USERNAME", username);
      command.ExecuteReader();
    }
  }
}
