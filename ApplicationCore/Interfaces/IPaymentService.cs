using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.DTOs.ProductDTOs;
using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface IPaymentService
    {
        Task<OperationResult<PayOrderByLineResponse>> PayOrderByLineAsync(int orderId);
        Task<OperationResult> ComfirmPaymentAsync(string transactionId, string orderNo);
    }
}
