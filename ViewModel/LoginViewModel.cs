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
    BitmapImage avatar = new BitmapImage(new Uri("ms-appx:///Assets/Login/Avatar.jpeg"));
    public BitmapImage Avatar {
      get { return avatar; }
      set { SetProperty(ref avatar, value); }
    }
    private string captcha = "";
    public string Captcha {
      get {
        return captcha;
      }
      set {
        SetProperty(ref captcha, value);
      }
    }
    private string username;
    public string Username {
      get { return username; }
      set { SetProperty(ref username, value); }
    }
    private string password;
    public string Password {
      get { return password; }
      set { SetProperty(ref password, value); }
    }

    private ObservableCollection<string> suggestions = new ObservableCollection<string>();
    public ObservableCollection<string> Suggestions {
      get { return suggestions; }
      set { SetProperty(ref suggestions, value); }
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
