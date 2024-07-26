using ApplicationCore.Models;
using EleganceParadisAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Helpers
{
    public static class OperationResultHelper
    {
        public static BadRequestObjectResult GetBadRequestResult<T>(this OperationResult<T> operationResult) where T : BaseOperationResult
        {
            if (operationResult.IsSuccess) throw new ArgumentException("operationResult.IsSuccess == true 不會回傳 BadRequestObjectResult");
            var errorResult = new BadRequestDTO
            {
                ErrorMessage = operationResult.ErrorMessage,
                Result = operationResult.ResultDTO
            };
            return new BadRequestObjectResult(errorResult);
        }
    }
}
