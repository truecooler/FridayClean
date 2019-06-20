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
		FridayCleanApiSettings Settings { get; }
		Task<AuthSendCodeResponse> AuthSendCodeAsync(AuthSendCodeRequest request);

		AuthSendCodeResponse AuthSendCode(AuthSendCodeRequest request);

		Task<AuthValidateCodeResponse> AuthValidateCodeAsync(AuthValidateCodeRequest request);

		AuthValidateCodeResponse AuthValidateCode(AuthValidateCodeRequest request);

		Task<AuthValidateTokenResponse> AuthValidateTokenAsync(AuthValidateTokenRequest request);

		AuthValidateTokenResponse AuthValidateToken(AuthValidateTokenRequest request);

		Task<GetProfileInfoResponse> GetProfileInfoAsync(GetProfileInfoRequest request);

		GetProfileInfoResponse GetProfileInfo(GetProfileInfoRequest request);

		Task<SetProfileInfoResponse> SetProfileInfoAsync(SetProfileInfoRequest request);

		SetProfileInfoResponse SetProfileInfo(SetProfileInfoRequest request);

		Task<UserLogoutResponse> UserLogoutAsync(UserLogoutRequest request);

		UserLogoutResponse UserLogout(UserLogoutRequest request);

		Task<GetCleaningServicesResponse> GetCleaningServicesAsync(GetCleaningServicesRequest request);

		GetCleaningServicesResponse GetCleaningServices(GetCleaningServicesRequest request);

		Task<GetOrderedCleaningsResponse> GetOrderedCleaningsAsync(GetOrderedCleaningsRequest request);

		GetOrderedCleaningsResponse GetOrderedCleanings(GetOrderedCleaningsRequest request);

		Task<OrderNewCleaningResponse> OrderNewCleaningAsync(OrderNewCleaningRequest request);

		OrderNewCleaningResponse OrderNewCleaning(OrderNewCleaningRequest request);

		Task<GetCleanersResponse> GetCleanersAsync(GetCleanersRequest request);

		GetCleanersResponse GetCleaners(GetCleanersRequest request);
	}
}
