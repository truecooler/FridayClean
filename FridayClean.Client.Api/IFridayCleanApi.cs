using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FridayClean.Client.Api
{
	public interface IFridayCleanApi : IDisposable
	{
		Task<AuthSendCodeResponse> AuthSendCodeAsync(AuthSendCodeRequest request, Metadata headers = null,
			DateTime? deadline = null, CancellationToken cancellationToken = default);

		AuthSendCodeResponse AuthSendCode(AuthSendCodeRequest request, Metadata headers = null,
			DateTime? deadline = null, CancellationToken cancellationToken = default);

		Task<AuthValidateCodeResponse> AuthValidateCodeAsync(AuthValidateCodeRequest request, Metadata headers = null,
			DateTime? deadline = null, CancellationToken cancellationToken = default);

		AuthValidateCodeResponse AuthValidateCode(AuthValidateCodeRequest request, Metadata headers = null,
			DateTime? deadline = null, CancellationToken cancellationToken = default);

		Task<AuthValidateTokenResponse> AuthValidateTokenAsync(AuthValidateTokenRequest request,
			Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);

		AuthValidateTokenResponse AuthValidateToken(AuthValidateTokenRequest request, Metadata headers = null,
			DateTime? deadline = null, CancellationToken cancellationToken = default);
	}
}
