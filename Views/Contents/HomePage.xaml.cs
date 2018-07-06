using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Matrix_UWP.Helpers;
using Newtonsoft.Json;
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
  public sealed partial class HomePage : Page, Helpers.INavigationViewContent {
    ViewModel.HomeViewModel viewModel = new ViewModel.HomeViewModel();
    public HomePage() {
      this.InitializeComponent();
    }

    public string GetTitle() { return "主页"; }
    public event NavigationViewContentHandler OnContentError;
    public event NavigationViewContentHandler OnContentLoading;
    public event NavigationViewContentHandler OnContentLoaded;
    public event NavigationViewContentHandler TitleChanged;

    public async Task Refresh() {
      OnContentLoading?.Invoke(this, new NavigationViewContentEvent());
      try {
        var todo = await Model.MatrixRequest.GetUnfinishAssignment();
        viewModel.TodoAssignments = todo.ToList();
      } catch (MatrixException.MatrixException err) {
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err));
      }
      OnContentLoaded?.Invoke(this, new NavigationViewContentEvent());
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      await Refresh();
      TitleChanged?.Invoke(this, new NavigationViewContentEvent(GetTitle()));
    }

    private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
      var assignment = (Model.Assignment)e.ClickedItem;
      var param = new {
        assignment.ca_id,
        assignment.course_id,
      };
      Frame.Navigate(typeof(AssignmentPage), JsonConvert.SerializeObject(param));
    }
  }
}
