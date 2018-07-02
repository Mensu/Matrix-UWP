using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.ViewModel {
  public class HomePageViewModel: BindableBase {
    private List<Model.Assignment> todoAssignments = new List<Model.Assignment>();
    public List<Model.Assignment> TodoAssignments {
      get => todoAssignments;
      set => SetProperty(ref todoAssignments, value);
    }
  }
}
