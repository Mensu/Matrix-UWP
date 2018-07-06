using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Matrix_UWP.ViewModel {
  class AssignmentViewModel : BindableBase {
    private List<Model.Submission> submissions = new List<Model.Submission>();
    public List<Model.Submission> Submissions {
      get => submissions;
      set => SetProperty(ref submissions, value);
    }

    private Model.Assignment assignment = new Model.Assignment();
    public Model.Assignment Assignment {
      get => assignment;
      set => SetProperty(ref assignment, value);
    }
  }
}
