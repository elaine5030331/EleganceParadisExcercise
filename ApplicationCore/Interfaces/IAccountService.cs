using ApplicationCore.DTOs.AccountDTOs;
using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface IAccountService
    {
        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="registInfo"></param>
        /// <returns></returns>
        Task<OperationResult<CreateAccountResponse>> CreateAccount(RegistDTO registInfo);

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<GetAccountInfoResponse> GetAccountInfo(int accountId);

        /// <summary>
        /// 更新個人資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<OperationResult<UpdateAcoountInfoResult>> UpdateAccountInfo(UpdateAccountInfoRequest request);

        /// <summary>
        /// 更新使用者密碼
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<OperationResult<UpdateAccountPasswordResult>> UpdateAccountPassword(UpdateAccountPasswordRequest request);

        /// <summary>
        /// 會員驗證信
        /// </summary>
        /// <param name="encodingParameter"></param>
        /// <returns></returns>
        Task<OperationResult<VerifyEmailResponse>> VerifyEmailAsync(string encodingParameter);

        /// <summary>
        /// 忘記密碼驗證信
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<OperationResult> ForgetPasswordAsync(string email);

        /// <summary>
        /// 驗證重設密碼
        /// </summary>
        /// <param name="encodingParameter"></param>
        /// <returns></returns>
        Task<OperationResult> VerifyForgetPasswordAsync(string encodingParameter);

        /// <summary>
        /// 重設密碼
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<OperationResult> ResetAccountPasswordAsync(ResetAccountPasswordRequest request);

        /// <summary>
        /// 重發註冊驗證信
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<OperationResult> ResendVerifyEmailAsync(ResendVerifyEmailRequest request);
    }
}