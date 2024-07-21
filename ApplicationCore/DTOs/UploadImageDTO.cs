using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs
{
    public class UploadImageDTO : BaseOperationResult
    {
        public string FileName { get; set; }
        public string URL { get; set; }
    }
}
