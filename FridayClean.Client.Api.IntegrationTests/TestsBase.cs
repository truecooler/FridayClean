using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FridayClean.Client.Api.IntegrationTests
{
	[TestClass]
	public class TestsBase
	{
		protected FridayCleanApi _api;
		protected string _validPhone = "79154013049";
		protected string _invalidPhone = "79154228013049";
		protected int _superValidCode = 00000;
		protected int _InvalidCode = 12345;

		protected string _invalidHost = "ololo.kek.com";
		protected string _invalidIp = "123.123.123.123";
		protected int _InvalidPort = 55555;
	}
}
