using Matrix_UWP.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
  public sealed partial class AssignmentPage : Page, Helpers.INavigationViewContent {
    public AssignmentPage() {
      this.InitializeComponent();
    }

    ViewModel.AssignmentViewModel viewModel = new ViewModel.AssignmentViewModel();

    public event NavigationViewContentHandler OnContentError;

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      JObject param = (JObject)JsonConvert.DeserializeObject((string)e.Parameter);
      int courseId = Helpers.Nullable.ToInt(param["CourseId"]);
      int caId = Helpers.Nullable.ToInt(param["CaId"]);
      await Refresh();
    }

    public async Task Refresh() {
      int courseId = viewModel.Assignment.course_id;
      int caId = viewModel.Assignment.ca_id;
      try {
        viewModel.Assignment = await Model.MatrixRequest.GetAssignment(courseId, caId);
      } catch (MatrixException.MatrixException err) {
        Debug.WriteLine($"获取课程{courseId}作业{caId}详情失败: {err.Message}");
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err));
      }
    }
  }
}
