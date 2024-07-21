using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CloudinaryService : IUploadImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<OperationResult<UploadImageDTO>> UploadImageAsync(IFormFile formFile)
        {
            var fileName = formFile.FileName;
            try
            {
                var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                {
                    File = new FileDescription(fileName, formFile.OpenReadStream()),
                    Folder = "EleganceParadis"
                });

                var dto = new UploadImageDTO()
                {
                    FileName = fileName,
                    URL = uploadResult.SecureUri.ToString()
                };
                var result = new OperationResult<UploadImageDTO>(dto);
               
                return result;

            }
            catch (Exception ex)
            {
                return new OperationResult<UploadImageDTO>()
                {
                    IsSuccess = false,
                    ErrorMessage = "圖片上傳API異常",
                    ResultDTO = new UploadImageDTO
                    {
                        FileName = fileName
                    }
                };
            }

        }
    }
}
