using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Mntone.SvgForXaml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Microsoft.Toolkit.Uwp.UI.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Matrix_UWP {
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page {
    ViewModel.LoginViewModel loginVm;
    public MainPage() {
      this.InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e) {
      bool isLogin = false;
      try {
        isLogin = await Model.MatrixRequest.isLogin();
      } catch (Exception err) {
        System.Diagnostics.Debug.WriteLine(err);
      }
      if (!isLogin) {
        Frame.Navigate(typeof(Views.Login));
      }
    }
    private async void HamburgerMenu_OnItemClick(object sender, ItemClickEventArgs e) {
      var menuItem = e.ClickedItem as HamburgerMenuItem;
      var dialog = new MessageDialog($"You clicked on {menuItem.Label} button");

      await dialog.ShowAsync();
    }
  }
}
