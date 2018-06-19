using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Matrix_UWP.ViewModel {
  class CourseListViewModel: BindableBase {
    private ObservableCollection<Model.Course> _closeList = new ObservableCollection<Model.Course>();
    public ObservableCollection<Model.Course> closeList {
      get { return _closeList; }
      set { SetProperty(ref _closeList, value); }
    }

    private ObservableCollection<Model.Course> _openList = new ObservableCollection<Model.Course>();
    public ObservableCollection<Model.Course> openList {
      get { return _openList; }
      set { SetProperty(ref _openList, value); }
    }

    private bool _isClose;
    public bool? isClose {
      get { return _isClose; }
      set { SetProperty(ref _isClose, value ?? false); }
    }

    public void Update(ObservableCollection<Model.Course> course_list) {
      openList = new ObservableCollection<Model.Course>(course_list.Where(one => one.isOpen == true));
      closeList = new ObservableCollection<Model.Course>(course_list.Where(one => one.isOpen == false));
    }

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
