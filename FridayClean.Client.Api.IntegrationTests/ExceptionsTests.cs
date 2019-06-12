using FridayClean.Client.Api.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FridayClean.Client.Api.IntegrationTests
{
	[TestClass]
	public class ExceptionsTests : TestsBase
	{
		[TestInitialize]
		public void TestInitialize()
		{
			//_api = new FridayCleanApi(new FridayCleanApiSettings(){Host = _invalidHost, Port = _InvalidPort});
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_api.Dispose();
		}

		[TestMethod]
		public void AuthSendCode_CreateInvalidHost_ExceptionThrown()
		{
			_api = new FridayCleanApi(new FridayCleanApiSettings() { Host = _invalidHost, Port = _InvalidPort });
			Assert.ThrowsException<GrpcUnavailableException>(() =>_api.AuthSendCode(new AuthSendCodeRequest() { Phone = _validPhone }));
		}

		[TestMethod]
		public void AuthSendCodeAsync_CreateInvalidHost_ExceptionThrown()
		{
			_api = new FridayCleanApi(new FridayCleanApiSettings() { Host = _invalidHost, Port = _InvalidPort });
			Assert.ThrowsExceptionAsync<GrpcUnavailableException>(() => _api.AuthSendCodeAsync(new AuthSendCodeRequest() { Phone = _validPhone }));
		}

		[TestMethod]
		public void AuthSendCode_CreateInvalidIp_ExceptionThrown()
		{
			_api = new FridayCleanApi(new FridayCleanApiSettings() { Host = _invalidIp, Port = _InvalidPort });
			Assert.ThrowsException<GrpcUnavailableException>(() => _api.AuthSendCode(new AuthSendCodeRequest() { Phone = _validPhone }));
		}

		[TestMethod]
		public void AuthSendCodeAsync_CreateInvalidIp_ExceptionThrown()
		{
			_api = new FridayCleanApi(new FridayCleanApiSettings() { Host = _invalidIp, Port = _InvalidPort });
			Assert.ThrowsExceptionAsync<GrpcUnavailableException>(() => _api.AuthSendCodeAsync(new AuthSendCodeRequest() { Phone = _validPhone }));
		}
	}
}
