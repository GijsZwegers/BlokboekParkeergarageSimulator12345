﻿using Newtonsoft.Json;
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

namespace BlokboekParkeergarageSimulator.CMS.Pages
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        private string url = null;
        ApiClass.ApiFuncionClass API = new ApiClass.ApiFuncionClass();
        JsonClasses.newtonsoftJSON Convert = new JsonClasses.newtonsoftJSON();
        private bool DeviceExist = false;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = null;

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer QRtimer = new System.Windows.Threading.DispatcherTimer();
        public StartPage()
        {
            InitializeComponent();
            UpdateLog();
            getCamList();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            QRtimer.Tick += new EventHandler(QRtimer_Tick);
            QRtimer.Interval += new TimeSpan(0, 0, 1);
        }



        //Event handlers
        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            btClose.IsEnabled = false;
            double angle = 90;

            rotateSlagboom(angle);

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
            }
            catch (ApplicationException)
            {
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
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            //do processing here
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {

                pbTest.Source = BitmapToImageSource(img);

            }
            ));
        }

        private void QRtimer_Tick(object sender, EventArgs e)
        {
            if (videoSource.IsRunning)
            {

            }
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
            lbAction.Content = "Device running... " + videoSource.FramesReceived.ToString() + " FPS";
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
                    lbAction.Content = "Device running...";
                    btStartCamera.Content = "Stop";
                    dispatcherTimer.Start();
                }
                else
                {
                    lbAction.Content = "Error: No Device selected.";
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
    }
    
}