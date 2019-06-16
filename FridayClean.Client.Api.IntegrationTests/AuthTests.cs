using Grpc.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FridayClean.Client.Api.IntegrationTests
{
	[TestClass]
	public class AuthTests : TestsBase
	{
		
		[TestInitialize]
		public void TestInitialize()
		{
			_api = new FridayCleanApi(FridayCleanApiSettings.ProductionDefault());
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_api.Dispose();
		}

		[TestMethod]
		public void AuthSendCode_SendCodeWithValidPhone_SmsDelivered()
		{
			var response = _api.AuthSendCode(new AuthSendCodeRequest() {Phone = _validPhone });
			Assert.AreEqual(AuthSendCodeStatus.Success,response.ResponseStatus);
		}

		

		[TestMethod]
		public void AuthSendCodeAsync_SendCodeWithValidPhone_SmsDelivered()
		{
			var response = _api.AuthSendCodeAsync(new AuthSendCodeRequest() { Phone = _validPhone }).GetAwaiter().GetResult();
			Assert.AreEqual(AuthSendCodeStatus.Success, response.ResponseStatus);
		}

		//TODO: Make tests with async version of api
		[TestMethod]
		public void AuthSendCode_SendCodeWithInvalidPhone_InvalidPhoneError()
		{
			var response = _api.AuthSendCode(new AuthSendCodeRequest() { Phone = _invalidPhone });
			Assert.AreEqual(AuthSendCodeStatus.InvalidPhone, response.ResponseStatus );
		}

		[TestMethod]
		public void AuthSendCodeAsync_SendCodeWithInvalidPhone_InvalidPhoneError()
		{
			var response = _api.AuthSendCodeAsync(new AuthSendCodeRequest() { Phone = _invalidPhone }).GetAwaiter().GetResult();
			Assert.AreEqual(AuthSendCodeStatus.InvalidPhone, response.ResponseStatus);
		}


		[TestMethod]
		public void AuthValidateCode_ValidateCode_InvalidCodeError()
		{
			var response = _api.AuthValidateCode(new AuthValidateCodeRequest(){Phone = _validPhone,Code = _InvalidCode});
			Assert.AreEqual(AuthValidateCodeStatus.InvalidCode,response.ResponseStatus);
		}


		[TestMethod]
		public void AuthValidateCodeAsync_ValidateCode_InvalidCodeError()
		{
			var response = _api.AuthValidateCodeAsync(new AuthValidateCodeRequest() { Phone = _validPhone, Code = _InvalidCode }).GetAwaiter().GetResult();
			Assert.AreEqual(AuthValidateCodeStatus.InvalidCode, response.ResponseStatus);
		}


		[TestMethod]
		public void AuthValidateCode_ValidateCode_ValidCode()
		{
			var response = _api.AuthValidateCode(new AuthValidateCodeRequest() { Phone = _validPhone, Code = _superValidCode });
			Assert.AreEqual(AuthValidateCodeStatus.ValidCode,response.ResponseStatus);
		}


		[TestMethod]
		public void AuthValidateCodeAsync_ValidateCode_ValidCode()
		{
			var response = _api.AuthValidateCodeAsync(new AuthValidateCodeRequest() { Phone = _validPhone, Code = _superValidCode }).GetAwaiter().GetResult();
			Assert.AreEqual(AuthValidateCodeStatus.ValidCode, response.ResponseStatus);
		}


		[TestMethod]
		public void AuthValidateTokenAsync_ValidateToken_InvalidToken()
		{
			var response = _api.AuthValidateTokenAsync(new AuthValidateTokenRequest() { AccessToken = "123"}).GetAwaiter().GetResult();
			Assert.AreEqual(AuthValidateTokenStatus.InvalidToken, response.ResponseStatus);
		}


	}
}
