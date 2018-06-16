using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Matrix_UWP.Views.Contents {
  /// <summary>
  /// 可用于自身或导航至 Frame 内部的空白页。
  /// </summary>
  public sealed partial class CoursesPage : Page {
    public CoursesPage() {
      this.InitializeComponent();
      NavigationCacheMode = NavigationCacheMode.Enabled;
    }

    private ViewModel.CourseListViewModel viewModel = new ViewModel.CourseListViewModel();

    private async void RefreshBtn_Click(object sender, RoutedEventArgs e) {
      try {
        var courseList = await Model.MatrixRequest.GetCourseList();
        viewModel.Courses = courseList.ToList();
      } catch (MatrixException.FatalError err) {
        Debug.WriteLine($"获取课程列表失败: {err.Message}");
      }
    }

    private void GridView_ItemClick(object sender, ItemClickEventArgs e) {
      var course = (Model.Course)e.ClickedItem;
      Debug.WriteLine($"转到课程{course.course_id}: {course.name}");
      //Frame.Navigate(typeof(CoursePage), course.course_id);
    }
  }
}
