using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Negosud.consts
{
    class Colors
    {
        public static Brush VALID_COLOR = new SolidColorBrush(Color.FromArgb(255, 0, 204, 68));
        public static Brush INVALID_COLOR = new SolidColorBrush(Color.FromArgb(255, 255, 77, 77));

        public static Brush ALERT_VALID_COLOR = new SolidColorBrush(Color.FromArgb(255, 26, 167, 160));
        public static Brush ALERT_INVALID_COLOR = new SolidColorBrush(Color.FromArgb(255, 167, 119, 160));
    }
}
