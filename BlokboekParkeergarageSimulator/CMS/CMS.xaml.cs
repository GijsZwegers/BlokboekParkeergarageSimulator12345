using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace BlokboekParkeergarageSimulator.CMS
{
    /// <summary>
    /// Interaction logic for CMS.xaml
    /// </summary>
    public partial class CMS : Window, INotifyPropertyChanged
    {
        private string url = null;
        ApiClass.ApiFuncionClass API = new ApiClass.ApiFuncionClass();
        JsonClasses.newtonsoftJSON Convert = new JsonClasses.newtonsoftJSON();
        Pages.StartPage MainPage = new Pages.StartPage();
        Pages.CameraPage Camerapage = new Pages.CameraPage();

        private string userInfo;
        public string UserInfo
        {
            get { return userInfo; }
            set
            {
                userInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserInfo)));
            }
        }

        public CMS()
        {
            InitializeComponent();
            DataContext = this;
            GetUserInfo();
            PageFrame.Navigate(new Pages.StartPage());
        }

        private void GetUserInfo()
        {
            url = "https://avondmtb.nl/middenpolder/user/me";
            string result = API.GETapiToken(url);
            JsonClasses.newtonsoftJSON.RootObject response = JsonConvert.DeserializeObject<JsonClasses.newtonsoftJSON.RootObject>(result);

            UserInfo = "Ingelogd als: "+ response.user.Name_First +" "+ response.user.Name_Middle + response.user.Name_Last;
            //ControlTemplate controlTemplate = tippy.Template;
            //((TextBlock)((StackPanel)((ControlTemplate)tippy.Template).Template).Children[0]).Text = "Test";
            /* ToolTip tip = new System.Windows.Controls.ToolTip();
            Middenpolder_Logo.ToolTip = tip;
            tip.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;*/
            
        }

        private void BTLogout_Click(object sender, RoutedEventArgs e)
        {
            Camerapage.CloseVideoSource();
            MainPage.CloseVideoSource();
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void btControl_Click(object sender, RoutedEventArgs e)
        {
            
            Camerapage.CloseVideoSource();
            PageFrame.Navigate(MainPage);
        }

        private void btCameraControl_Click(object sender, RoutedEventArgs e)
        {
            
            MainPage.CloseVideoSource();
            PageFrame.Navigate(Camerapage);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Camerapage.CloseVideoSource();
            MainPage.CloseVideoSource();
        }
        private void Window_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void btPersoneel_Click(object sender, RoutedEventArgs e)
        {
            Camerapage.CloseVideoSource();
            MainPage.CloseVideoSource();

        }
    }
}