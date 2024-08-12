using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.ProductDTOs
{
    public class AddProductResponse : BaseOperationResult
    {
        public int ProductId { get; set; }
    }
}
