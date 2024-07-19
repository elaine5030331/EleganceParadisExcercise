using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces.DTOs
{
    public class AuthenticateDTO
    {
        public int AccountId { get; set; }
        public bool IsValid { get; set; }
    }
}
