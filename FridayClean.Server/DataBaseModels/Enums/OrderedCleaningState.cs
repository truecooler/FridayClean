using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridayClean.Server.DataBaseModels.Enums
{
	public enum OrderedCleaningState
	{

		WaitingForCleanerСonfirmation,
		ClientIsWaitingForCleanerArrival,
		CleanerWorkInProgress,
		Completed

	}
}
