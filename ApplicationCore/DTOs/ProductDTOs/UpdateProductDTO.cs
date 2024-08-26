using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.ProductDTOs
{
    public class UpdateProductDTO
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string SPU { get; set; }
        public string ProductName { get; set; }
        public bool Enable { get; set; }
        public string Description { get; set; }
    }
}
