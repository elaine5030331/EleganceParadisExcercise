using ApplicationCore.Constants;
using ApplicationCore.DTOs.ProductDTOs;
using ApplicationCore.DTOs.SpecDTOs;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecController : ControllerBase
    {
        private readonly ISpecService _specService;

        public SpecController(ISpecService specService)
        {
            _specService = specService;
        }
    }
}
