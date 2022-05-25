using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;


namespace FaceNetCore4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Face.face();
        }
        String police_photo = "";

        private void add_img_bt_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "图像文件|*.jpg;*.png;*.jpeg;*.bmp;*.gif|所有文件|*.*",
                RestoreDirectory = true, //设置对话框是否记忆之前打开的目录
                Title = "请选择照片",
            };
            if ((bool)openFile.ShowDialog())
            {
                try
                {
                    photo.Source = GetImage(openFile.FileName);

                    //photo.Source = new BitmapImage(new Uri(openFile.FileName));
                    police_photo = openFile.FileName;
                }
                catch
                {
                    MessageBox.Show("添加照片失败！");
                }

            }
        }

        public static BitmapImage GetImage(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();
            if (File.Exists(imagePath))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                {
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
            }
            return bitmap;
        }

        private void add_bt_Click(object sender, RoutedEventArgs e)
        {
            string police = pid.Text;
            string user_info = "police";
            string group_id = "team";

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            FaceCompare detect = new FaceCompare();
            // 人脸usb摄像头检测
            detect.usb_video_identify();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Detect detect = new Detect();
            detect.video_detect_identify();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //new WebServer().server();
            //CreateWebHostBuilder().Build().Run();
            var host = new WebHostBuilder()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .UseKestrel()
               .UseStartup<Startup>()
               .Build();

            host.Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder() =>
            WebHost.CreateDefaultBuilder()
         .UseStartup<Startup>();

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            FaceCrop faceCrop=new FaceCrop();
            faceCrop.test_get_face_crop();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Detect detect = new Detect();
            detect.video_feature();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            Detect detect = new Detect();
            detect.deserialize_identify();
        }
    }
}
