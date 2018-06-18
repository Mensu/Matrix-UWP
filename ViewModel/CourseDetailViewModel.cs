using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.ViewModel {
  class CourseDetailViewModel: BindableBase {
    private int courseId = 0;
    public int CourseId {
      get => courseId;
      set => SetProperty(ref courseId, value);
    }

    private Model.Course course = new Model.Course();
    public Model.Course Course {
      get => course;
      set {
        SetProperty(ref course, value);
      }
    }

    private List<Model.Assignment> assignments = new List<Model.Assignment>();
    public List<Model.Assignment> Assignments {
      get => assignments;
      set {
        SetProperty(ref assignments, value);
      }
    }

    private List<Model.User> users = new List<Model.User>();
    public List<Model.User> Users {
      get => users;
      set {
        SetProperty(ref users, value);
      }
    }
  }
}
