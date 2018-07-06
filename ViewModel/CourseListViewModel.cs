using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Matrix_UWP.ViewModel {
  class CourseListViewModel: BindableBase {

    private List<Model.Course> courses = new List<Model.Course>();
    public List<Model.Course> Courses {
      get => courses;
      set {
        SetProperty(ref courses, value);
        base.RaisePropertyChanged("ClosedCourses");
        base.RaisePropertyChanged("OpenCourses");
      }
    }

    public List<Model.Course> OpenCourses {
      get => courses.Where(course => course.isOpen).ToList();
    }

    public List<Model.Course> ClosedCourses {
      get => courses.Where(course => !course.isOpen).ToList();
    }
  }
}
