using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FridayClean.Client.Api.IntegrationTests
{
	[TestClass]
	public class AuthTests
	{
		private FridayCleanApi _api;
		private string _validPhone = "79154013049";
		private string _invalidPhone = "79154228013049";
		private int _superValidCode = 00000;
		private int _InvalidCode = 12345;

		[TestInitialize]
		public void TestInitialize()
		{
			_api = new FridayCleanApi(FridayCleanApiSettings.Default());
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_api.Dispose();
		}

		[TestMethod]
		public void Api_SendCodeWithValidPhone_SmsDelivered()
		{
			var response = _api.AuthSendCode(new AuthSendCodeRequest() {Phone = _validPhone });
			Assert.AreEqual(AuthSendCodeResponseStatus.Success,response.ResponseStatus);
		}

		//TODO: Make tests with async version of api
		[TestMethod]
		public void Api_SendCodeWithInvalidPhone_InvalidPhoneError()
		{
			var response = _api.AuthSendCode(new AuthSendCodeRequest() { Phone = _invalidPhone });
			Assert.AreEqual(AuthSendCodeResponseStatus.InvalidPhone, response.ResponseStatus );
		}


		[TestMethod]
		public void Api_ValidateCode_InvalidCodeError()
		{
			var response = _api.AuthValidateCode(new AuthValidateCodeRequest(){Phone = _validPhone,AuthCode = _InvalidCode});
			Assert.AreEqual(AuthValidateCodeResponseStatus.InvalidCode,response.ResponseStatus);
		}


		[TestMethod]
		public void Api_ValidateCode_ValidCode()
		{
			var response = _api.AuthValidateCode(new AuthValidateCodeRequest() { Phone = _validPhone, AuthCode = _superValidCode });
			Assert.AreEqual(AuthValidateCodeResponseStatus.ValidCode,response.ResponseStatus);
		}
	}
}
