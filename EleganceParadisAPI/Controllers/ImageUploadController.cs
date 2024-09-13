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
        /// <remarks>
        ///Sample response:<br/>
        ///     {
        ///        "fileName": "piggy.jpg",
        ///        "url": "https://res.cloudinary.com/dupxtirfd/image/upload/v1726213331/EleganceParadis/thqa5pyzwsk8qdapfi1z.jpg"
        ///     }
        /// </remarks>
        /// <response code ="200">上傳圖片成功</response>
        /// <response code ="200">
        /// 1. 檔案上傳格式有誤
        /// 2. 圖片上傳API異常
        /// </response>
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
