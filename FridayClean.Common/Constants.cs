using System;
using System.Collections.Generic;
using System.Text;

namespace FridayClean.Common
{
	public static class Constants
	{
		public const string AuthHeaderName = "interceptor-access-token";
		public const string UserPhoneHeaderName = "interceptor-user-phone";
		public const string AccessTokenSecureStorageKey = "fridayclean-access-token";


		public static class Messages
		{
			public const string UnableToCallRpcMessage =
				"Ошибка: Невозможно выполнить запрос. Возможно, сервер недоступен, попробуйте позже.";

			public const string SecureStorageNotSupportedMessage =
				"Ошибка: Ваш телефон не поддерживает SecureStorage! После перезапуска приложения придется авторизоваться заного.";
		}
	}
}
