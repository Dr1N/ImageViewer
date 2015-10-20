using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gallery
{
    public class TemplateSelector : DataTemplateSelector
    {
        public FrameworkElement Window { get; set; }
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
           //FrameworkElement element = container as FrameworkElement;
           //Window w = (Window)element.Parent;
           Console.WriteLine("Selector");
           return Window.FindResource("TreeViewDriveDataTemplate") as DataTemplate;
        }
    }
}
