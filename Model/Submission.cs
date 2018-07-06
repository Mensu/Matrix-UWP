using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.Model {
  public class Submission: BindableBase {
    public Submission(JToken token = null) {
      if (token is JObject submission) {
        Grade = submission["grade"]?.ToObject<int>();
        SubCaId = Helpers.Nullable.ToInt(submission["sub_ca_id"]);
        SubAsgnId = Helpers.Nullable.ToInt(submission["sub_asgn_id"]);
        SubmitAt = Helpers.Nullable.ToDateTimeOffset(submission["submit_at"], DateTimeOffset.Now);
        UserId = Helpers.Nullable.ToInt(submission["user_id"]);
      }
    }

    private int? grade;
    public int? Grade {
      get => grade;
      set => SetProperty(ref grade, value);
    }

    private int sub_asgn_id;
    public int SubAsgnId {
      get => sub_asgn_id;
      set => SetProperty(ref sub_asgn_id, value);
    }

    private int sub_ca_id;
    public int SubCaId {
      get => sub_ca_id;
      set => SetProperty(ref sub_ca_id, value);
    }

    private DateTimeOffset submit_at;
    public DateTimeOffset SubmitAt {
      get => submit_at;
      set => SetProperty(ref submit_at, value);
    }

    private int user_id;
    public int UserId {
      get => user_id;
      set => SetProperty(ref user_id, value);
    }
  }
}
