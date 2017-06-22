using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media.Animation;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Threading;
using AForge.Video;
using com.google.zxing.qrcode;
using com.google.zxing.qrcode.detector;
using COMMON = com.google.zxing.common;
using com.google.zxing;
using System.Windows.Input;
using System.Windows.Threading;

namespace BlokboekParkeergarageSimulator.CMS.Pages
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        private string url = null;
        private string json = null;
        private string result = null;
        ApiClass.ApiFuncionClass API = new ApiClass.ApiFuncionClass();
        JsonClasses.newtonsoftJSON Convert = new JsonClasses.newtonsoftJSON();
        private bool DeviceExist = false;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = null;
        public QRCodeReader reader = new QRCodeReader();
        public Bitmap current { get; private set; }
        Bitmap img;
        DispatcherTimer idchecker = new DispatcherTimer();

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer QRtimer = new System.Windows.Threading.DispatcherTimer();
        public StartPage()
        {
            InitializeComponent();
            double angle = 90;
            Functions.BoardWrapper.ZetLampjeAan(2);
            rotateSlagboom(angle);
            UpdateLog();
            getCamList();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            EventManager.RegisterClassHandler(typeof(Window),
            Keyboard.KeyUpEvent, new KeyEventHandler(Page_KeyDown), true);

            idchecker.Tick += new EventHandler(idchecker_Tick);
            idchecker.Interval = new TimeSpan(0, 0, 0, 1);
            idchecker.Start();
        }

        private void idchecker_Tick(object sender, EventArgs e)
        {
            bool[] Channel = Functions.BoardWrapper.magnet();
            if(Channel[4])
            {
                Functions.BoardWrapper.ZetLampjeAan(2);
            }
        }

        //Event handlers
        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            btClose.IsEnabled = false;
            double angle = 90;
            
            rotateSlagboom(angle);
            Functions.BoardWrapper.ZetLampjeAan(2);
            url = "https://avondmtb.nl/middenpolder/log/logaction?LogNummer=2";
            string result = API.GETapiToken(url);
            UpdateLog();
            
            btOpen.IsEnabled = true;
        }

        private void btOpen_Click(object sender, RoutedEventArgs e)
        {
            btOpen.IsEnabled = false;
            double angle = 0;

            rotateSlagboom(angle);
            Functions.BoardWrapper.ZetLampjeAan(1);

            url = "https://avondmtb.nl/middenpolder/log/logaction?LogNummer=1";

            string result = API.GETapiToken(url);
                
            UpdateLog();

            btClose.IsEnabled = true;
            
        }

        private void rbManual_Checked(object sender, RoutedEventArgs e)
        {
            url = "https://avondmtb.nl/middenpolder/log/logaction?LogNummer=5";

            string result = API.GETapiToken(url);

            UpdateLog();
             
        }

        private void rbAutomatic_Checked(object sender, RoutedEventArgs e)
        {
            url = "https://avondmtb.nl/middenpolder/log/logaction?LogNummer=4";

            string result = API.GETapiToken(url);
            //automatisch de log updaten 
            UpdateLog();

        }


        //Zelf geschreven functies 

        //datagrid sorteren op de eerste column
        public static void SortDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Descending)
        {
            var column = dataGrid.Columns[columnIndex];

            dataGrid.Items.SortDescriptions.Clear();

            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));

            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;

            dataGrid.Items.Refresh();
        }
        //slagboom draaien dmv animatie 
        private void rotateSlagboom(double angle)
        {

            var animation = new DoubleAnimation
            {
                To = angle,
                Duration = TimeSpan.FromSeconds(0.7)
            };

            var transform = SlagboomAngel;

            transform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }
        //Log updaten
        private void UpdateLog()
        {
            url = "https://avondmtb.nl/middenpolder/log/getlog";

            string result = API.GETapiToken(url);
            JsonClasses.newtonsoftJSON.RootObject response = JsonConvert.DeserializeObject<JsonClasses.newtonsoftJSON.RootObject>(result);
            dgLog.IsReadOnly = true;
            dgLog.ItemsSource = response.log.ToList();

            SortDataGrid(dgLog);



        }

        private void getCamList()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                cbCameraSelect.Items.Clear();
                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                DeviceExist = true;
                foreach (FilterInfo device in videoDevices)
                {
                    cbCameraSelect.Items.Add(device.Name);
                }
                cbCameraSelect.SelectedIndex = 0;
                btStartCamera.Visibility = Visibility.Visible;
            }
            catch (ApplicationException)
            {
                btStartCamera.Visibility = Visibility.Hidden;
                DeviceExist = false;
                cbCameraSelect.Items.Add("No capture device on your system");
            }
        }


        //Bitmapimage omzetten zodat het in wpf gebruikt kan worden
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        //eventhandler if new frame is ready
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            img = (Bitmap)eventArgs.Frame.Clone();
            //do processing here
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {

                pbTest.Source = BitmapToImageSource(img);
            }
            ));
        }


        //close the device safely
        public void CloseVideoSource()
        {
            lbAction.Content = "Device stopped.";
            btStartCamera.Content = "Start";
            dispatcherTimer.Stop();
            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }
            pbTest.Source = null;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // code goes here
            lbAction.Content = "Apparaat in gebruik... " + videoSource.FramesReceived.ToString() + " FPS";
        }

        private void btStartCamera_Click_1(object sender, RoutedEventArgs e)
        {
            if (btStartCamera.Content.ToString() == "Start")
            {
                if (DeviceExist)
                {
                    videoSource = new VideoCaptureDevice(videoDevices[cbCameraSelect.SelectedIndex].MonikerString);
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                    CloseVideoSource();
                    //videoSource.DesiredFrameRate = 10;
                    videoSource.Start();
                    lbAction.Content = "Apparaat in gebruik...";
                    btStartCamera.Content = "Stop";
                    dispatcherTimer.Start();
                }
                else
                {
                    lbAction.Content = "Error: Er is geen camera gevonden.";
                }
            }
            else
            {
                if (videoSource.IsRunning == true)
                {
                    CloseVideoSource();
                }
            }
        }

        private void CamRefresh_Click(object sender, RoutedEventArgs e)
        {
            getCamList();
        }


        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            var isNumber = e.Key >= Key.D0 && e.Key <= Key.D9;
            var arr = e.Key.ToString().ToCharArray();
            if (isNumber)
            {
                tbWerkNemer.Text = tbWerkNemer.Text + arr[1].ToString();
            }
            if (e.Key == Key.Enter)
            {
                
                url = "https://avondmtb.nl/middenpolder/user/user?BarCode="+tbWerkNemer.Text;
                tbWerkNemer.Text = "";
                string result = API.GETapiToken(url);
                JsonClasses.newtonsoftJSON.RootObject response = JsonConvert.DeserializeObject<JsonClasses.newtonsoftJSON.RootObject>(result);
                if (response.success)
                {
                    /*url = "https://avondmtb.nl/middenpolder/log/loguser";
                    json = "{\"LogNummer\": \"3\",    \"Client\": \""+response.user.Name_First+" "+ response.user.Name_Last+"\"}";
                    result = API.POSTapiToken(url, json);
                    */
                    rotateSlagboom(0);
                    btClose.IsEnabled = true;
                    btOpen.IsEnabled = false;
                    lbLaatsteUser.Text = "Gebruiker " + response.user.Name_First +" "+ response.user.Name_Last+" is de parkeerplaats opgereden";
                    Functions.BoardWrapper.ZetLampjeAan(1);

                    UpdateLog();
                }
            }
        }

        private void btBlokkeergebruiker_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("De gebruiker is geblokkeerd");
        }
    }
    
}
