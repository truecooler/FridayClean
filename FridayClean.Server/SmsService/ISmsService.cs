using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridayClean.Server.SmsService
{
	public interface ISmsService
	{
		AuthSendCodeStatus SendSms(string number, string message);
		Task<AuthSendCodeStatus> SendSmsAsync(string number, string message);
	}
}
