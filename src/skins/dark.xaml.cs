using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gallery
{
    public partial class dark
    {
        //Двойной клик по папке в дереве
        private void tv_directories_itemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount >= 2)
            {
                MainWindow mainWindow = (MainWindow)((App)Application.Current).WindowRef;
                mainWindow.AddDirectory();
                e.Handled = true;
            }
        }

        //Двойной клик по папке в списке папок
        private void lb_selectedDirectories_itemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount >= 2)
            {
                MainWindow mainWindow = (MainWindow)((App)Application.Current).WindowRef;
                mainWindow.RemoveDirectory();
            }
        }

        //Контекстное меню - добавить каталог (Add Directory)
        private void MenuItem_Add_Directory_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)((App)Application.Current).WindowRef;
            mainWindow.AddDirectory();
        }

        //Контекстное меню - удалить каталог (Remove Directory)
        private void MenuItem_Remove_Directory_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)((App)Application.Current).WindowRef;
            mainWindow.RemoveDirectory();
        }

        //Контекстное меню удалить всё (Remove All) спискa директорий
        private void MenuItem_RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)((App)Application.Current).WindowRef;
            if (mainWindow.selectedDirectories.Count != 0)
            {
                mainWindow.selectedDirectories.Clear();
                mainWindow.lb_selectedDirectories.Items.Refresh();
                mainWindow.imageFiles.Clear();
                mainWindow.lb_imageList.Items.Refresh();
            }
        }
    }
}
