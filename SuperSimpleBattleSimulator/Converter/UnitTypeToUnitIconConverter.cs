using SuperSimpleBattleSimulator.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SuperSimpleBattleSimulator.Converter
{
    public class UnitTypeToUnitIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var unitType = (UnitType)value;
            string icon_name = "";
            switch (unitType)
            {
                case UnitType.Cavalary:
                    icon_name = "horse";
                    break;
                case UnitType.Infantry:
                    icon_name = "sword_single";
                    break;
                case UnitType.Ranged:
                    icon_name = "archery";
                    break;
            }
            return Application.Current.Resources[icon_name];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
