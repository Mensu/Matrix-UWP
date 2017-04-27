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
    public MainPage() {
      this.InitializeComponent();
      this.CreateHttpClient();
      this.DataContext = this;
    }

    private HttpClient httpClient;
    private async void btn_Click(object sender, RoutedEventArgs e) {
      var form = new JObject();
      form["username"] = this.usernameInput.Text;
      form["password"] = this.passwordInput.Password;
      if (this.captchaInput.Text.Length > 0) {
        form["captcha"] = this.captchaInput.Text;
      }
      IHttpContent jsonContent = new HttpJsonContent(form);
      var response = await httpClient.PostAsync(new Uri("https://vmatrix.org.cn/api/users/login"), jsonContent);
      var text = await response.Content.ReadAsStringAsync();
      Debug.WriteLine($"POST Response: {text}");
      this.textBlock.Text += text + "\n";
      JObject obj = JsonConvert.DeserializeObject(text) as JObject;
      updateSvg(obj);
    }

    private void CreateHttpClient() {
      if (this.httpClient != null) {
        this.httpClient.Dispose();
      }

      // HttpClient functionality can be extended by plugging multiple filters together and providing
      // HttpClient with the configured filter pipeline.
      IHttpFilter filter = new HttpBaseProtocolFilter();
      filter = new PlugInFilter(filter); // Adds a custom header to every request and response message.
      this.httpClient = new HttpClient(filter);

      string ua = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
      if (!this.httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(ua)) {
        Debug.WriteLine("Failed to use Chrome User Agent");
      }

    }

    private void updateSvg(JObject json) {
      var svg = json["data"]["captcha"];
      if (svg == null) {
        Debug.WriteLine("captcha == null");
        return;
      }
      string svgText = svg.ToString();
      this.svgImg.Content = SvgDocument.Parse(svgText);
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e) {
      var response = await httpClient.GetAsync(new Uri("https://vmatrix.org.cn/api/captcha"));
      string text = await response.Content.ReadAsStringAsync();
      Debug.WriteLine($"GET Response: {text}");
      this.textBlock.Text += text + "\n";
      JObject obj = JsonConvert.DeserializeObject(text) as JObject;
      updateSvg(obj);
    }
  }

  public sealed class PlugInFilter : IHttpFilter {
    private IHttpFilter innerFilter;

    public PlugInFilter(IHttpFilter innerFilter) {
      if (innerFilter == null) {
        throw new ArgumentException("innerFilter cannot be null.");
      }
      this.innerFilter = innerFilter;
    }

    public IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> SendRequestAsync(HttpRequestMessage request) {
      return AsyncInfo.Run<HttpResponseMessage, HttpProgress>(async (cancellationToken, progress) => {
        Uri requestUri = request.RequestUri;

        HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
        HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(requestUri);

        string text = cookieCollection.Count + " cookies found.\r\n";
        foreach (HttpCookie cookie in cookieCollection) {
          text += "--------------------\r\n";
          text += "Name: " + cookie.Name + "\r\n";
          text += "Domain: " + cookie.Domain + "\r\n";
          text += "Path: " + cookie.Path + "\r\n";
          text += "Value: " + cookie.Value + "\r\n";
          text += "Expires: " + cookie.Expires + "\r\n";
          text += "Secure: " + cookie.Secure + "\r\n";
          text += "HttpOnly: " + cookie.HttpOnly + "\r\n";
          if (cookie.Name == "X-CSRF-Token") {
            Debug.WriteLine("token added");
            request.Headers.Add(cookie.Name, cookie.Value);
          }
        }

        Debug.WriteLine(text);

        request.Headers.Add("Custom-Header", "CustomRequestValue");
        HttpResponseMessage response = await innerFilter.SendRequestAsync(request).AsTask(cancellationToken, progress);

        cancellationToken.ThrowIfCancellationRequested();

        response.Headers.Add("Custom-Header", "CustomResponseValue");
        return response;
      });
    }

    public void Dispose() {
      innerFilter.Dispose();
      GC.SuppressFinalize(this);
    }
  }

  class HttpJsonContent : IHttpContent {
    JObject json;
    HttpContentHeaderCollection headers;

    public HttpContentHeaderCollection Headers {
      get {
        return headers;
      }
    }

    public HttpJsonContent(JObject json) {
      this.json = json;
      headers = new HttpContentHeaderCollection();
      headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
      headers.ContentType.CharSet = "UTF-8";
    }

    public IAsyncOperationWithProgress<ulong, ulong> BufferAllAsync() {
      return AsyncInfo.Run<ulong, ulong>((cancellationToken, progress) =>
      {
        return Task<ulong>.Run(() =>
        {
          ulong length = GetLength();

          // Report progress.
          progress.Report(length);

          // Just return the size in bytes.
          return length;
        });
      });
    }

    public IAsyncOperationWithProgress<IBuffer, ulong> ReadAsBufferAsync() {
      return AsyncInfo.Run<IBuffer, ulong>((cancellationToken, progress) =>
      {
        return Task<IBuffer>.Run(() =>
        {
          DataWriter writer = new DataWriter();
          writer.WriteString(JsonConvert.SerializeObject(json));

          // Make sure that the DataWriter destructor does not free the buffer.
          IBuffer buffer = writer.DetachBuffer();

          // Report progress.
          progress.Report(buffer.Length);

          return buffer;
        });
      });
    }

    public IAsyncOperationWithProgress<IInputStream, ulong> ReadAsInputStreamAsync() {
      return AsyncInfo.Run<IInputStream, ulong>(async (cancellationToken, progress) =>
      {
        InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
        DataWriter writer = new DataWriter(randomAccessStream);
        writer.WriteString(JsonConvert.SerializeObject(json));

        uint bytesStored = await writer.StoreAsync().AsTask(cancellationToken);

        // Make sure that the DataWriter destructor does not close the stream.
        writer.DetachStream();

        // Report progress.
        progress.Report(randomAccessStream.Size);

        return randomAccessStream.GetInputStreamAt(0);
      });
    }

    public IAsyncOperationWithProgress<string, ulong> ReadAsStringAsync() {
      return AsyncInfo.Run<string, ulong>((cancellationToken, progress) =>
      {
        return Task<string>.Run(() =>
        {
          string jsonString = JsonConvert.SerializeObject(json);

          // Report progress (length of string).
          progress.Report((ulong)jsonString.Length);

          return jsonString;
        });
      });
    }

    public bool TryComputeLength(out ulong length) {
      length = GetLength();
      return true;
    }

    public IAsyncOperationWithProgress<ulong, ulong> WriteToStreamAsync(IOutputStream outputStream) {
      return AsyncInfo.Run<ulong, ulong>(async (cancellationToken, progress) =>
      {
        DataWriter writer = new DataWriter(outputStream);
        writer.WriteString(JsonConvert.SerializeObject(json));
        uint bytesWritten = await writer.StoreAsync().AsTask(cancellationToken);

        // Make sure that DataWriter destructor does not close the stream.
        writer.DetachStream();

        // Report progress.
        progress.Report(bytesWritten);

        return bytesWritten;
      });
    }

    public void Dispose() {
    }

    private ulong GetLength() {
      DataWriter writer = new DataWriter();
      writer.WriteString(JsonConvert.SerializeObject(json));

      IBuffer buffer = writer.DetachBuffer();
      return buffer.Length;
    }
  }
}
