using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace SuperSimpleBattleSimulator.Converter
{
    public class ItemClickEventArgsToClickedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = value as ItemClickEventArgs;
            return args?.ClickedItem;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
