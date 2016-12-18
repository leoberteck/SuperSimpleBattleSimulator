using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SuperSimpleBattleSimulator.Converter
{
    public class HealthToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var health = (double)value;
            string color = "#FF48FA4F";
            if (health >=10 && health < 30)
            {
                color = "#FFFAFF21";
            }
            else if (health < 10) {
                color = "#FFFF2121";
            }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
