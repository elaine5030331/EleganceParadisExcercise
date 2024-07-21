using ApplicationCore.DTOs;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IUploadImageService
    {
        public Task<OperationResult<UploadImageDTO>> UploadImageAsync(IFormFile formFile);
    }
}
