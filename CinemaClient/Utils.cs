using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CinemaClient
{
    public sealed class Utils
    {
        public static void ShowError(Window context, string message)
        {
            MessageBox.Show(context, message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfo(Window context, string message)
        {
            MessageBox.Show(context, message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static bool Ask(Window context, string message)
        {
            return MessageBox.Show(context, message, "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }
    }
}
