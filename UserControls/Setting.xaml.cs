using Matrix_UWP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_UWP.UserControls {
  public sealed partial class Setting : UserControl, IHamburgerContent {
    internal ViewModel.SettingViewModel vm = new ViewModel.SettingViewModel();
    public Setting() {
      this.InitializeComponent();
    }

    public event HamburgerContentHandler onError;

    public async Task ResetContentAsync() {
      try {
        this.vm.curUser = await Model.MatrixRequest.getProfile();
      } catch (MatrixException.NotLogin err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
        // 返回登录页面？
      } catch (MatrixException.MatrixException err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
      }
    }

    private async void Button_Click(object sender, RoutedEventArgs e) {
      try {
        await Model.MatrixRequest.changeProfile(this.nicknameInput.Text, "", "", "");
      } catch (MatrixException.NotLogin err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
        // 返回登录页面？
      } catch (MatrixException.MatrixException err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
      }
    }
  }
}
