using Matrix_UWP.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Matrix_UWP.Views.Contents {
  /// <summary>
  /// 可用于自身或导航至 Frame 内部的空白页。
  /// </summary>
  public sealed partial class CourseDetailPage : Page, Helpers.INavigationViewContent {
    public CourseDetailPage() {
      this.InitializeComponent();
      NavigationCacheMode = NavigationCacheMode.Required;
    }

    private ViewModel.CourseDetailViewModel viewModel = new ViewModel.CourseDetailViewModel();

    public string GetTitle() { return viewModel.Course.name; }
    public event NavigationViewContentHandler OnContentError;
    public event NavigationViewContentHandler OnContentLoading;
    public event NavigationViewContentHandler OnContentLoaded;
    public event NavigationViewContentHandler TitleChanged;

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      if (e.NavigationMode == NavigationMode.Back) return;
      viewModel.CourseId = (int)e.Parameter;
      await Refresh();
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e) {
      await Refresh();
    }

    public async Task Refresh() {
      OnContentLoading?.Invoke(this, new NavigationViewContentEvent());
      try {
        viewModel.Course = await Model.MatrixRequest.GetCourse(viewModel.CourseId);
        viewModel.Assignments = await Model.MatrixRequest.GetAssignmentList(viewModel.CourseId);
      } catch (MatrixException.MatrixException err) {
        Debug.WriteLine($"请求课程信息错误：{err.Message}");
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err));
      }
      OnContentLoaded?.Invoke(this, new NavigationViewContentEvent());

      TitleChanged?.Invoke(this, new NavigationViewContentEvent(GetTitle()));
    }

    private void AssignmentList_ItemClick(object sender, ItemClickEventArgs e) {
      var assignment = (Model.Assignment)e.ClickedItem;
      var param = new {
        assignment.ca_id,
        assignment.course_id,
      };
      Frame.Navigate(typeof(AssignmentPage), JsonConvert.SerializeObject(param));
    }
  }
}
