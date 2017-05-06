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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Matrix_UWP {
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page {
    ViewModel.LoginViewModel loginVm;
    public MainPage() {
      this.InitializeComponent();
      this.DataContext = this.loginVm = new ViewModel.LoginViewModel();
    }

    private async void btn_Click(object sender, RoutedEventArgs e) {
      string username = this.usernameInput.Text;
      string password = this.passwordInput.Password;
      string captcha = "";
      if (this.captchaInput.Text.Length > 0) {
        captcha = this.captchaInput.Text;
      }
      this.captchaInput.Text = "";
      this.loginVm.captcha = new Model.Captcha();
      Model.User curUser;
      try {
        curUser = await Model.MatrixRequest.login(username, password, captcha);
      } catch (MatrixException.FatalError err) {
        this.textBlock.Text += $"致命错误: {err.Message}\n";
        return;
      } catch (MatrixException.WrongCaptcha err) {
        updateSvg(err.captcha);
        this.loginVm.captcha = err.captcha;
        return;
      } catch (MatrixException.WrongPassword err) {
        this.passwordInput.Password = "";
        this.textBlock.Text += $"不沃克: {err.Message}\n";
        return;
      } catch (MatrixException.SoftError err) {
        this.textBlock.Text += $"不沃克: {err.Message}\n";
        return;
      }
      this.textBlock.Text += "沃克\n";
      try {
        this.textBlock.Text += await Model.MatrixRequest.getCourseList();
      } catch (MatrixException.FatalError err) {
        this.textBlock.Text += $"不沃克: {err.Message}\n";
      }
      this.textBlock.Text += "又沃克\n";
    }

    private void updateSvg(Model.Captcha captcha = null) {
      //this.svgImg.Content = SvgDocument.Parse((captcha ?? new Model.Captcha()).svg);
      //this.captchaInput.Visibility = Visibility.Visible;
      //this.svgImg.Visibility = Visibility.Visible;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e) {
      //this.updateSvg();
      this.loginVm.captcha = new Model.Captcha();
      //this.captchaInput.Visibility = Visibility.Collapsed;
      //this.svgImg.Visibility = Visibility.Collapsed;
    }
  }
}
