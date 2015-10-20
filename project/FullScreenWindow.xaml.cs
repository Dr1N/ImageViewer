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
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Gallery
{
    /// <summary>
    /// Логика взаимодействия для FullScreenWindow.xaml
    /// </summary>
    public partial class FullScreenWindow : Window
    {
        public List<FileInfo> imageFiles = new List<FileInfo>();
        private int current = 0;
        private DispatcherTimer MyTimer = new DispatcherTimer();
        private List<Storyboard> storyboards = new List<Storyboard>();
        private Random rand = new Random();

        public FullScreenWindow()
        {
            InitializeComponent();
            this.KeyDown += FullScreenWindow_KeyDown;
            this.Loaded += FullScreenWindow_Loaded;

            this.storyboards.Add((Storyboard)this.FindResource("story1"));
            this.storyboards.Add((Storyboard)this.FindResource("story2"));
            this.storyboards.Add((Storyboard)this.FindResource("story3"));

            this.MyTimer.Interval = TimeSpan.FromMilliseconds(2000);
            this.MyTimer.Tick += MyTimer_Tick;
        }

        void FullScreenWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MyTimer.Start();
        }

        void MyTimer_Tick(object sender, EventArgs e)
        {
            //Загрузка картинки
            
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(this.imageFiles[this.current].FullName, UriKind.Relative);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            
            this.img_currentImage.Source = image;
            this.current++;
            if (this.current == this.imageFiles.Count)
            {
                this.MyTimer.Stop();
                this.Close();
            }

            //Показ анимации
            int animation = this.rand.Next(0, this.storyboards.Count);
            this.storyboards[animation].Begin();
        }

        private void FullScreenWindow_KeyDown(object sender, KeyEventArgs e)
        {
            this.MyTimer.Stop();
            this.Close();
        }
    }
}
