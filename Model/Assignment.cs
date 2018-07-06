using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;

namespace Matrix_UWP.Model {
  public class Assignment : BindableBase {
    public Assignment(JToken token = null) {
      if (token == null) {
        token = new JObject();
      }
      var data = token as JObject;
      this.course_id = Helpers.Nullable.ToInt(data["course_id"]);
      this.ca_id = Helpers.Nullable.ToInt(data["ca_id"]);
      this.startDate = Helpers.Nullable.ToDateTimeOffset(data["startdate"], DateTimeOffset.MinValue);
      this.endDate = Helpers.Nullable.ToDateTimeOffset(data["enddate"], DateTimeOffset.MinValue);
      this.name = Helpers.Nullable.ToString(data["title"]);
      this.course_name = Helpers.Nullable.ToString(data["courseName"]);
      this.description = Helpers.Nullable.ToString(data["description"]);
      this.creator = new User(data["author"]);
      this.student_num_waiting_for_judge = Helpers.Nullable.ToInt(data["stuNumWaitingForJudging"]);
      this.total_student_num = Helpers.Nullable.ToInt(data["totalStuNum"]);
      this.type = this.getType(data["type"] ?? "");

      if (IsProgramming && data.ContainsKey("config")) {
        var config = data["config"];
        string lang = Helpers.Nullable.ToString(config["standard_language"], "text");
        var files = data["files"] as JArray;
        var submissions = config["submission"] as JArray;
        List<CodeFile> assignmentFiles = new List<CodeFile>();
        foreach (JToken submission in submissions) {
          CodeFile submissionFile = new CodeFile(Helpers.Nullable.ToString(submission, "未知"), lang);
          assignmentFiles.Add(submissionFile);
          this.Submissions.Add(submissionFile);
        }
        foreach (JToken file in files) {
          assignmentFiles.Add(new CodeFile(file, lang));
        }
        Files = assignmentFiles;
      }
    }

    public Task<int> SubmitProgramming() {
      return MatrixRequest.SubmitProgramming(course_id, ca_id, Submissions);
    }

    public bool IsProgramming {
      get => type == Type.RealtimeProgramming || type == Type.ProgrammingProblem || type == Type.ScheduleProgramming;
    }

    private List<CodeFile> files = new List<CodeFile>();
    public List<CodeFile> Files {
      get => files;
      set => SetProperty(ref files, value);
    }

    public List<CodeFile> Submissions { get; set; } = new List<CodeFile>();

    public enum Type {
      RealtimeProgramming,
      ScheduleProgramming,
      ProgrammingProblem,
      Choice,
      Report,
      FileUpload,
      ProgrammingOutput,
      ProgrammingFillingBlank,
      ShortAnswer
    }

    public int course_id {
      get;
    }

    public int ca_id {
      get;
    }

    private DateTimeOffset _startDate;
    public DateTimeOffset startDate {
      get {
        return this._startDate;
      }
      set {
        this.SetProperty(ref this._startDate, value);
      }
    }

    private DateTimeOffset _endDate;
    public DateTimeOffset endDate {
      get {
        return this._endDate;
      }
      set {
        this.SetProperty(ref this._endDate, value);
      }
    }

    private string _name;
    public string name {
      get {
        return this._name;
      }
      set {
        this.SetProperty(ref this._name, value);
      }
    }

    private string _description;
    public string description {
      get {
        return this._description;
      }
      set {
        this.SetProperty(ref this._description, value);
      }
    }

    private User _creator;
    public User creator {
      get {
        return this._creator;
      }
      set {
        this.SetProperty(ref this._creator, value);
      }
    }

    private Type _type;
    public Type type {
      get {
        return this._type;
      }
      set {
        this.SetProperty(ref this._type, value);
      }
    }

    private int _total_student_num;
    public int total_student_num {
      get {
        return _total_student_num;
      }
      set {
        SetProperty(ref _total_student_num, value);
        RaisePropertyChanged("student_num_judged");
      }
    }

    private int _student_num_waiting_for_judge;
    public int student_num_waiting_for_judge {
      get {
        return _student_num_waiting_for_judge;
      }
      set {
        SetProperty(ref _student_num_waiting_for_judge, value);
        RaisePropertyChanged("student_num_judged");
      }
    }

    public int student_num_judged {
      get {
        return _total_student_num - _student_num_waiting_for_judge;
      }
    }

    private string _course_name;
    public string course_name {
      get {
        return _course_name;
      }
      set {
        SetProperty(ref _course_name, value);
      }
    }

    private Type getType(JToken token) {
      switch (token.ToString()) {
        case "Programming problem":
          return Type.ProgrammingProblem;
        case "Realtime Programming":
          return Type.RealtimeProgramming;
        case "Schedule Programming":
          return Type.ScheduleProgramming;
        case "Choice problem":
          return Type.Choice;
        case "Report":
          return Type.Report;
        case "Fileupload problem":
          return Type.FileUpload;
        case "Program Output problem":
          return Type.ProgrammingOutput;
        case "Program Blank Filling problem":
          return Type.ProgrammingFillingBlank;
        default:
          break;
      }
      return Type.ShortAnswer;
    }
    static readonly public Assignment Null = new Assignment();
  }
}
