using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SuperSimpleBattleSimulator.Converter
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            object ret = Visibility.Collapsed;
            if((bool)value == true)
            {
                ret = Visibility.Visible;
            }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            object ret = false;
            if ((Visibility)value == Visibility.Visible)
            {
                ret = true;
            }
            return ret;
        }
    }
}
