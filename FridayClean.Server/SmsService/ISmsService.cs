using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridayClean.Server.SmsService
{
	public interface ISmsService
	{
		AuthSendCodeResponseStatus SendSms(string number, string message);
		Task<AuthSendCodeResponseStatus> SendSmsAsync(string number, string message);
	}
}
