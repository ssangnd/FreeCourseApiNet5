using FreeCourseApiNet5.Services;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourseApiNet5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IHangHoaRepository _hangRepository;

        public ProductsController(IHangHoaRepository hangHoaRepository)
        {
            _hangRepository = hangHoaRepository;
        }

        [HttpGet]
        public IActionResult GetAllProducts(string search, double? from, double? to, string sortBy, int page=1)
        {
            try
            {
                var result = _hangRepository.GetAll(search,from,to,sortBy,page);
                return Ok(result);
            }
            catch
            {
                return BadRequest("We can not get product list");
            }
        }
    }
}
