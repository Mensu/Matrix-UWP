using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Matrix_UWP.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Matrix_UWP.Views.Contents {
  /// <summary>
  /// 可用于自身或导航至 Frame 内部的空白页。
  /// </summary>
  public sealed partial class CoursesPage : Page, Helpers.INavigationViewContent {
    public CoursesPage() {
      this.InitializeComponent();
      NavigationCacheMode = NavigationCacheMode.Required;
    }

    private ViewModel.CourseListViewModel viewModel = new ViewModel.CourseListViewModel();

    public event NavigationViewContentHandler OnContentError;
    public event NavigationViewContentHandler OnContentLoading;
    public event NavigationViewContentHandler OnContentLoaded;
    public event NavigationViewContentHandler TitleChanged;

    private async void RefreshBtn_Click(object sender, RoutedEventArgs e) {
      await Refresh();
    }

    private void GridView_ItemClick(object sender, ItemClickEventArgs e) {
      var course = (Model.Course)e.ClickedItem;
      Debug.WriteLine($"转到课程{course.course_id}: {course.name}");
      Frame.Navigate(typeof(CourseDetailPage), course.course_id);
    }

    public async Task Refresh() {
      OnContentLoading?.Invoke(this, new NavigationViewContentEvent());
      try {
        var courseList = await Model.MatrixRequest.GetCourseList();
        viewModel.Courses = courseList.ToList();
      } catch (MatrixException.MatrixException err) {
        Debug.WriteLine($"获取课程列表失败: {err.Message}");
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err));
      }
      OnContentLoaded?.Invoke(this, new NavigationViewContentEvent());
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      await Refresh();
      TitleChanged?.Invoke(this, new NavigationViewContentEvent("课程列表"));
    }
  }
}
