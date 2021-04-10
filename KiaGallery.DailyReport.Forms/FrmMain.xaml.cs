using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KiaGallery.DailyReport.Forms
{
    /// <summary>
    /// Interaction logic for FrmMain.xaml
    /// </summary>
    public partial class FrmMain
    {
        public FrmMain()
        {
            //  Common.Cache.CurrentUser = new Common.Model.User() { token = "0a22f4e9-af61-446f-8fc8-7f7f8ca821ed", lastName = "Manny" };

            InitializeComponent();

            System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
            col.Add("token", Common.Cache.CurrentUser.token);

            KiaGallery.Common.Response rep = Common.Services.CallService(Common.Services.ServiceType.GetBaseData, col);
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private async void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {
            //var sampleMessageDialog = new SampleMessageDialog
            //{
            //    Message = { Text = ((ButtonBase)sender).Content.ToString() }
            //};

            await DialogHost.Show("", "RootDialog");
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

            if (Common.Cache.CurrentUser != null)
                txtUserTitle.Text = Common.Cache.CurrentUser.firsName + " " + Common.Cache.CurrentUser.lastName + " (" + Common.Cache.CurrentUser.branch + ")";
        }
    }
}
