using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SuperSimpleBattleSimulator.Converter
{
    public class IntToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double res = 0;
            double.TryParse(value?.ToString(), out res);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int res = 0;
            int.TryParse(value?.ToString(), out res);
            return res;
        }
    }
}
