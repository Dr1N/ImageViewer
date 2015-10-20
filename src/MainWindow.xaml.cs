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
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Gallery
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Параметры

        private string[] imageFileExtention = { ".bmp", ".jpg", ".jpeg", ".png", ".gif", ".tif" };
        string[] themes = { "Light(default)", "Dark" };

        #endregion

        #region Поля(данные)

        public List<DirectoryInfo> selectedDirectories = new List<DirectoryInfo>();
        public List<FileInfo> imageFiles = new List<FileInfo>();
        private DispatcherTimer MyTimer;
        private List<Storyboard> storyboards = new List<Storyboard>();
        private Random rand = new Random();

        #endregion

        #region Конструтор
       
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            
            //Список тем
            
            foreach (var item in this.themes)
            {
                this.cb_themes.Items.Add(item);
            }

            this.cb_themes.SelectedIndex = 0;

            //Набор анимаций

            this.storyboards.Add((Storyboard)this.FindResource("story1"));
            this.storyboards.Add((Storyboard)this.FindResource("story2"));
            this.storyboards.Add((Storyboard)this.FindResource("story3"));

            this.lb_selectedDirectories.Items.Refresh();
            this.lb_imageList.Items.Refresh();
        }

        #endregion

        #region Обработка событий окна

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.fillTreeView();
            
            //Привязка данных
            this.lb_selectedDirectories.ItemsSource = this.selectedDirectories;
            this.lb_imageList.ItemsSource = this.imageFiles;

            //Установка ссылки на окно
            App currentApp = (App)Application.Current;
            currentApp.WindowRef = this;
        }

        #endregion

        #region Обработка событий меню
       
        private void mn_exit_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mn_about_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Super-Puper Gallery", "Gallery", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //Контекстное меню - добавить каталог (Add Directory)
        private void MenuItem_Add_Directory_Click(object sender, RoutedEventArgs e)
        {
            this.AddDirectory();
        }

        //Контекстное меню - удалить каталог (Remove Directory)
        private void MenuItem_Remove_Directory_Click(object sender, RoutedEventArgs e)
        {
            this.RemoveDirectory();
        }

        //Контекстное меню удалить всё (Remove All) списка директорий
        private void MenuItem_RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedDirectories.Count != 0)
            {
                this.selectedDirectories.Clear();
                this.lb_selectedDirectories.Items.Refresh();
                this.imageFiles.Clear();
                this.lb_imageList.Items.Refresh();
            }
        }

        #endregion

        #region Обработка ToolBar
        
        private void cb_themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex != -1)
            {
                ResourceDictionary rs = new ResourceDictionary();
                if (cb.SelectedIndex == 0)
                {
                    rs.Source = new Uri("skins/light.xaml", UriKind.Relative);
                }
                else
                {
                    rs.Source = new Uri("skins/dark.xaml", UriKind.Relative);
                }
                this.Resources.MergedDictionaries[0] = rs;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Test");
            FullScreenWindow fullScreen = new FullScreenWindow();
            fullScreen.ShowDialog();
        }

        #endregion

        #region Навигация

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            if (this.imageFiles.Count == 0) { return; }
            if (this.lb_imageList.SelectedIndex == 0) { return; }
            if (this.lb_imageList.SelectedIndex == -1)
            {
                this.lb_imageList.SelectedIndex = this.lb_imageList.Items.Count - 1;
            }
            else
            {
                this.lb_imageList.SelectedIndex--;
            }
            this.lb_imageList.ScrollIntoView(this.lb_imageList.SelectedItem);
        }

        private void btn_forward_Click(object sender, RoutedEventArgs e)
        {
            if (this.imageFiles.Count == 0) { return; }
            if(this.lb_imageList.SelectedIndex == this.lb_imageList.Items.Count - 1) { return; }
            if (this.lb_imageList.SelectedIndex == -1)
            {
                this.lb_imageList.SelectedIndex = 0;
            }
            else
            {
                this.lb_imageList.SelectedIndex++;
            }
            this.lb_imageList.ScrollIntoView(this.lb_imageList.SelectedItem);
        }

        private void btn_play_Click(object sender, RoutedEventArgs e)
        {
            if (this.MyTimer != null && this.MyTimer.IsEnabled) { return; }

            if (this.imageFiles.Count == 0) { return; }

            if (this.cb_fullScreen.IsChecked == true)
            {
                FullScreenWindow fullScreen = new FullScreenWindow();
                fullScreen.imageFiles = this.imageFiles;
                fullScreen.ShowDialog();
            }
            else
            {
                this.cb_fullScreen.Visibility = System.Windows.Visibility.Collapsed;
                this.MyTimer = new DispatcherTimer();
                this.MyTimer.Interval = TimeSpan.FromMilliseconds(2000);
                this.MyTimer.Tick += MyTimer_Tick;

                if (this.lb_imageList.SelectedIndex == -1)
                {
                    this.lb_imageList.SelectedIndex = 0;
                }

                this.MyTimer.Start();
            }
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            this.cb_fullScreen.Visibility = System.Windows.Visibility.Visible;
            this.MyTimer.Stop();
        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            if (this.imageFiles.Count == 0 || this.lb_imageList.SelectedIndex >= this.imageFiles.Count) 
            {
                this.MyTimer.Stop();
                return;
            }

            //Загрузка картинки

            if (this.lb_imageList.SelectedIndex == this.imageFiles.Count - 1)
            {
                this.lb_imageList.SelectedIndex = 0;
            }
            else
            {
                this.lb_imageList.SelectedIndex++;
            }
            this.lb_imageList.ScrollIntoView(this.lb_imageList.SelectedItem);
            
            //Показ анимации

            int animation = this.rand.Next(0, this.storyboards.Count);
            this.storyboards[animation].Begin();
        }

        #endregion

        #region Обработка событий дерева каталогов

        private void tv_directories_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            if (item.Parent is TreeView) { return; }
            item.Items.Clear();
            DirectoryInfo dir;
            if (item.Tag is DriveInfo)
            {
                DriveInfo drive = (DriveInfo)item.Tag;
                dir = drive.RootDirectory;
            }
            else
            {
                dir = (DirectoryInfo)item.Tag;
            }
            try
            {
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    TreeViewItem newItem = new TreeViewItem();
                    newItem.Header = subDir.ToString();
                    this.setTreeViewItemHeaderTemplate(newItem, "TreeViewDirectoryDataTemplate");
                    newItem.Tag = subDir;
                    if(this.isExpanded(subDir))
                    {
                        newItem.Items.Add("*");
                    }
                    item.Items.Add(newItem);
                }
            }
            catch
            { }
        }

        private void tv_directories_Collapsed(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            if (!(item.Parent is TreeView))
            {
                DirectoryInfo dir;
                if (item.Tag is DriveInfo)
                {
                    DriveInfo drive = (DriveInfo)item.Tag;
                    dir = drive.RootDirectory;
                }
                else
                {
                    dir = (DirectoryInfo)item.Tag;
                }
                item.Items.Clear();
                if (this.isExpanded(dir))
                {
                    item.Items.Add("*");
                }
            }
        }

        //Выделить элемент по нажатию правой кнопкой, перед показом контекстного меню
        private void tv_directories_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && e.Source is TreeViewItem)
            {
                TreeViewItem tvi = (TreeViewItem)e.Source;
                tvi.IsSelected = true;
            }
        }

        //Двойной клик по папке в дереве
        private void tv_directories_itemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                this.AddDirectory();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Проверка на наличие в директории вложенных дирикторий
        /// </summary>
        /// <param name="di">Директория</param>
        /// <returns>true - вложенные директории есть, false - дирикторий нет</returns>
        private bool isExpanded(DirectoryInfo di)
        {
            try
            {
                if (di.GetDirectories().Length > 0)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Установка шаблона для заголовка элемента дерева
        /// </summary>
        /// <param name="tvi">Элемент дерева</param>
        /// <param name="styleName">Имя стиля</param>
        private void setTreeViewItemHeaderTemplate(TreeViewItem tvi, string styleName)
        {
            try
            {
                DataTemplate headerDataTemplate = new DataTemplate();
                headerDataTemplate = (DataTemplate)this.FindResource(styleName);
                tvi.HeaderTemplate = headerDataTemplate;
            }
            catch { }
        }

        /// <summary>
        /// Заполнение дерева доступными дисками системы
        /// </summary>
        private void fillTreeView()
        {
            TreeViewItem root = new TreeViewItem();
            root.Header = "Компьютер";
            this.setTreeViewItemHeaderTemplate(root, "TreeViewRootDataTemplate");
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = drive.ToString();
                    this.setTreeViewItemHeaderTemplate(item, "TreeViewDriveDataTemplate");
                    item.Tag = drive;
                    if (this.isExpanded(drive.RootDirectory))
                    {
                        item.Items.Add("*");
                    }
                    root.Items.Add(item);
                }
            }

            this.tv_directories.Items.Add(root);
        }

        #endregion

        #region Обработка событий списка выбранных каталогов
        
        //Двойной клик по папке в списке папок
        private void lb_selectedDirectories_itemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                this.RemoveDirectory();
            }
        }

        private void lb_selectedDirectories_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (this.selectedDirectories.Count == 0)
            {
                e.Handled = true;
            }
        }
        
        #endregion

        #region Helpers

        /// <summary>
        /// Добавление выбранной папки в дереве катлогов к списку просматриваемых
        /// </summary>
        public void AddDirectory()
        {
            if (this.tv_directories.SelectedItem == null) { return; }

            try
            {
                //Добавление папки в список

                TreeViewItem tvi = (TreeViewItem)this.tv_directories.SelectedItem;
                DirectoryInfo di = (DirectoryInfo)tvi.Tag;
                foreach (DirectoryInfo item in this.selectedDirectories)
                {
                    if (di.Equals(item)) { return; }
                }
                this.selectedDirectories.Add(di);
                
                //Добавление файлов в список

                foreach (FileInfo file in di.GetFiles())
                {
                    if (this.imageFileExtention.Contains(file.Extension.ToLower()))
                    {
                        this.imageFiles.Add(file);
                    }
                }
            }
            catch { }
            finally
            {
                this.lb_selectedDirectories.Items.Refresh();
                this.lb_imageList.Items.Refresh();
            }
        }

        /// <summary>
        /// Удаляет выбранные папки в списке для просмотра
        /// </summary>
        public void RemoveDirectory()
        {
            //Удалить выбранные папки
            foreach (DirectoryInfo item in this.lb_selectedDirectories.SelectedItems)
            {
                this.selectedDirectories.Remove(item);
                //Удалить файлы этого каталога из списка
                this.RemoveDirectoryFilesFromList(item);
            }
            this.lb_imageList.Items.Refresh();
            this.lb_selectedDirectories.Items.Refresh();
        }

        /// <summary>
        /// Удалить фафлы из списка файлов, что принадлежат удалённой директории
        /// </summary>
        /// <param name="item">Удаляемая директория из списка</param>
        public void RemoveDirectoryFilesFromList(DirectoryInfo directory)
        {
            try
            {
                this.imageFiles.RemoveAll(file => {
                    bool result = false;
                    foreach (FileInfo item in directory.GetFiles())
                    {
                        if (file.FullName == item.FullName)
                        {
                            result = true;
                            break;
                        }
                    }
                    return result; 
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        #endregion
    }
}
