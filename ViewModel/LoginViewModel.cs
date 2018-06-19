using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Windows.UI.Xaml.Media.Imaging;

namespace Matrix_UWP.ViewModel {
  class LoginViewModel : BindableBase {
    BitmapImage _avatar = new BitmapImage(new Uri("ms-appx:///Assets/Login/Avatar.jpeg"));
    public BitmapImage avatar {
      get { return _avatar; }
      set { SetProperty(ref _avatar, value); }
    }
    private string _captcha = "";
    public string captcha {
      get {
        return this._captcha;
      }
      set {
        SetProperty(ref _captcha, value);
      }
    }
    private string _username;
    public string username {
      get { return _username; }
      set { SetProperty(ref _username, value); }
    }
    private string _password;
    public string password {
      get { return _password; }
      set { SetProperty(ref _password, value); }
    }

    private ObservableCollection<string> _suggestions = new ObservableCollection<string>();
    public ObservableCollection<string> suggestions {
      get { return _suggestions; }
      set { SetProperty(ref _suggestions, value); }
    }

    private SvgImageSource captchaSource = new SvgImageSource();
    public SvgImageSource CaptchaSource {
      get => captchaSource;
      set => SetProperty(ref captchaSource, value);
    }

    private bool needCaptcha = false;
    public bool NeedCaptcha {
      get => needCaptcha;
      set => SetProperty(ref needCaptcha, value);
    }
  }
}
