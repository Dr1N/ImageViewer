using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gallery
{
    public class ImageHeigthConverter : IValueConverter
    {
        public double Ratio { get; set; }

        public ImageHeigthConverter() { }

        public ImageHeigthConverter(double ratio)
        {
            this.Ratio = ratio;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double val;
            try
            {
                val = (Double)value;
                return val * this.Ratio;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}