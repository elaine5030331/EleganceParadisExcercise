using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using EleganceParadisAPI.DTOs;
using EleganceParadisAPI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly IUploadImageService _imageService;
        public ImageUploadController(IUploadImageService imageService)
        {
            _imageService = imageService;
        }

        /// <summary>
        /// 上傳圖片
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadIamge(List<IFormFile> files)
        {
            if (files == null || files.Count == 0 || files.Any(x => !ImageFileValidator.IsValidateExtensions(x.FileName)))
            {
                return BadRequest("檔案上傳格式有誤");
            }
            var result = new List<UploadIamgeResponseDTO>();
            foreach (var file in files)
            {
                var uploadResult = await _imageService.UploadImageAsync(file);
                result.Add(new UploadIamgeResponseDTO()
                {
                    IsSuccess = uploadResult.IsSuccess,
                    ErrorMessage = uploadResult.ErrorMessage,
                    FileName = uploadResult.ResultDTO.FileName,
                    URL = uploadResult.ResultDTO.URL
                });
            }
            return Ok(result);
        }

    }
}
