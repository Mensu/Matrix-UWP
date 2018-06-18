using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Matrix_UWP.Helpers;
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
  public sealed partial class LibrariesPage : Page, Helpers.INavigationViewContent {
    public LibrariesPage() {
      this.InitializeComponent();
    }

    public event NavigationViewContentHandler OnContentError;
    public event NavigationViewContentHandler OnContentLoading;
    public event NavigationViewContentHandler OnContentLoaded;
    public event NavigationViewContentHandler TitleChanged;

    public async Task Refresh() {
      // notify start loading
      OnContentLoading?.Invoke(this, new NavigationViewContentEvent());
      try {
        var libraries = await Model.MatrixRequest.GetLibraryList();
        viewModel.Libraries = libraries.ToList();
      } catch (MatrixException.MatrixException err) {
        var message = "获取题库列表失败";
        Debug.WriteLine($"{message}: {err.Message}");
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err, message));
      }
      // notify loaded end
      OnContentLoaded?.Invoke(this, new NavigationViewContentEvent());
    }

    ViewModel.LibrariesViewModel viewModel = new ViewModel.LibrariesViewModel();

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      await Refresh();

      TitleChanged?.Invoke(this, new NavigationViewContentEvent("题库列表"));
    }

    private void LibrariesPanel_ItemClick(object sender, ItemClickEventArgs e) {
      var library = (Model.Library)e.ClickedItem;
      // Todo: Library Detail
    }
  }
}
