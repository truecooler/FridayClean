using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FridayClean.Client.Converters
{
	public class OrderedCleaningStateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			OrderedCleaningState type = (OrderedCleaningState)value;
			switch (type)
			{
				case OrderedCleaningState.Canceled:
					return "Отменен";
				case OrderedCleaningState.CleanerWorkInProgress:
					return "Идет уборка";
				case OrderedCleaningState.Completed:
					return "Завершен";
				case OrderedCleaningState.WaitingForCleanerArrival:
					return "Клинер в пути";
				case OrderedCleaningState.WaitingForCleanerConfirmation:
					return "Ожидание подтверждения клинера";
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
