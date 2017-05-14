using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Matrix_UWP.Helpers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_UWP.UserControls {
  public sealed partial class Home : UserControl, IHamburgerContent {
    ViewModel.HomeViewModel vm = new ViewModel.HomeViewModel();
    public Home() {
      this.InitializeComponent();
    }

    public event HamburgerContentHandler onError;

    public async Task ResetContentAsync() {
      try {
        await vm.update();
      } catch (Exception err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
      }
    }

    private async void MatrixInsert_LinkClicked(object sender, LinkClickedEventArgs e) {
      await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
    }
  }
}
