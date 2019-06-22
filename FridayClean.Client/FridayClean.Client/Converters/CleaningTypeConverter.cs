using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FridayClean.Client.Converters
{

	public class CleaningTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			CleaningType type = (CleaningType)value;
			switch (type)
			{
				case CleaningType.ComplexCleaning:
					return "Комплексная";
				case CleaningType.GeneralCleaning:
					return "Генеральная";
				case CleaningType.MaintenanceCleaning:
					return "Поддерживающая";
				default:
					return "Неизвестно";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
