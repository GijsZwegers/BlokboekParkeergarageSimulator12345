using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using AForge.Vision.Motion;

namespace BlokboekParkeergarageSimulator.CMS.Pages
{

    /// <summary>
    /// Interaction logic for CameraPage.xaml
    /// </summary>
    public partial class CameraPage : Page
    {
        private bool DeviceExist = false;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = null;
        private Bitmap capturedImage;
        private String message = "";

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public CameraPage()
        {
            InitializeComponent();
            getCamList();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

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
        private void btStartCamera_Click(object sender, EventArgs e)
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
    }
}
