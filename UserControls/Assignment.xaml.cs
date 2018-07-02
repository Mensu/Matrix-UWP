using System;
using System.Threading.Tasks;
using Matrix_UWP.Helpers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MarkdownSharp;
using Windows.ApplicationModel.DataTransfer;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_UWP.UserControls {
  public sealed partial class Assignment : UserControl, Helpers.IHamburgerContent {
    internal ViewModel.AssignmentViewModel vm = new ViewModel.AssignmentViewModel();
    public Assignment() {
      this.InitializeComponent();
      this.listColView.DataContext = vm;
      this.detailColView.DataContext = vm;
      DataTransferManager dataTransferMananger = DataTransferManager.GetForCurrentView();
      dataTransferMananger.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.shareAssignment);

    }


    public event HamburgerContentHandler onError;

    public async Task ResetContentAsync() {
      var courseList = await Model.MatrixRequest.GetCourseList();
      await refreshList(59);
    }

    public async Task refreshList(int course_id) {
      var newList = this.vm.progressingList;
      bool success = false;
      this.vm.listIsLoading = true;
      try {
        newList = await Model.MatrixRequest.GetAssignmentList(course_id);
        success = true;
      } catch (MatrixException.SoftError err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
        return;
      } catch (MatrixException.FatalError err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
        return;
      } finally {
        this.vm.listIsLoading = false;
      }
      if (!success) {
        return;
      }
      this.vm.updateWith(newList);
    }

    private async void listView_OnItemClicked(object sender, AssignmentList.AssignmentItemClickEventArgs e) {
      this.vm.detailIsLoading = true;
      try {
        this.vm.curAssignment = await Model.MatrixRequest.GetAssignment(e.course_id, e.ca_id);
      } catch (MatrixException.SoftError err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
        return;
      } catch (MatrixException.FatalError err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
        return;
      } finally {
        this.vm.detailIsLoading = false;
      }
    }

    private void detailColView_OnShared(object sender, RoutedEventArgs e) {
      if (vm.curAssignment == Model.Assignment.Null) return ;
      DataTransferManager.ShowShareUI();
    }
    private void shareAssignment(DataTransferManager sender, DataRequestedEventArgs args) {

      string desc = vm.curAssignment.description;
      DateTimeOffset end = vm.curAssignment.endDate;
      var options = new MarkdownOptions {
        AutoHyperlink = true,
        LinkEmails = true,
      };
      Markdown md = new Markdown(options);
      string html_description = md.Transform(desc);
      DataRequest request = args.Request;
      request.Data.Properties.Title = "Assignment Shared from Matrix_UWP";
      request.Data.Properties.Description = vm.curAssignment.name;
      string html_content = HtmlFormatHelper.CreateHtmlFormat($"<h1>DDL: {end.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss")}</h1><div>{html_description}</div>");
      System.Diagnostics.Debug.WriteLine(html_content);
      request.Data.SetHtmlFormat(html_content);
    }
  }
}
