using Matrix_UWP.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Matrix_UWP.Views.Contents {
  /// <summary>
  /// 可用于自身或导航至 Frame 内部的空白页。
  /// </summary>
  public sealed partial class AssignmentPage : Page, Helpers.INavigationViewContent {
    public AssignmentPage() {
      this.InitializeComponent();
      NavigationCacheMode = NavigationCacheMode.Required;
    }

    ViewModel.AssignmentViewModel viewModel = new ViewModel.AssignmentViewModel();


    public string GetTitle() { return viewModel.Assignment.name; }
    public event NavigationViewContentHandler OnContentError;
    public event NavigationViewContentHandler OnContentLoading;
    public event NavigationViewContentHandler OnContentLoaded;
    public event NavigationViewContentHandler TitleChanged;

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      JObject param = (JObject)JsonConvert.DeserializeObject((string)e.Parameter);
      viewModel.Assignment = new Model.Assignment(param);
      await Refresh();
    }

    public async Task Refresh() {
      int courseId = viewModel.Assignment.course_id;
      int caId = viewModel.Assignment.ca_id;

      // notify loading
      OnContentLoading?.Invoke(this, new NavigationViewContentEvent());

      try {
        viewModel.Assignment = await Model.MatrixRequest.GetAssignment(courseId, caId);
        viewModel.Submissions = await Model.MatrixRequest.GetSubmissionList(courseId, caId);
      } catch (MatrixException.MatrixException err) {
        var message = $"获取课程{courseId}作业{caId}详情失败";
        Debug.WriteLine($"{message}: {err.Message}");
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err, message));
      }

      // notify loaded
      OnContentLoaded?.Invoke(this, new NavigationViewContentEvent());

      // set title since assignment name changed.
      TitleChanged?.Invoke(this, new NavigationViewContentEvent(GetTitle()));
    }

    private async void SubmitAssignment(object sender, RoutedEventArgs e) {
      try {
        await viewModel.Assignment.SubmitProgramming();
      } catch (MatrixException.MatrixException err) {
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err));
      }
    }
  }
}
