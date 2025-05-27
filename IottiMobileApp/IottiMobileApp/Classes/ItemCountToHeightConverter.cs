using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace IottiMobileApp.Classes
{
    class ItemCountToHeightConverter : IValueConverter
    {
        public double ItemHeight { get; set; } = 50; // Altezza di ogni item
        public double MaxHeight { get; set; } = 250; // Altezza massima
        public double MinHeight { get; set; } = 100; // Altezza minima

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                double calculatedHeight = count * ItemHeight;

                // Applica i limiti
                if (calculatedHeight < MinHeight)
                    return MinHeight;

                if (calculatedHeight > MaxHeight)
                    return MaxHeight;

                return calculatedHeight;
            }

            return MinHeight;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
