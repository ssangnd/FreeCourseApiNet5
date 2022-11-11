using FreeCourseApiNet5.Data;
using FreeCourseApiNet5.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FreeCourseApiNet5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoaiController : ControllerBase
    {
        private readonly MyDbContext _context;

        public LoaiController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var dsLoai = _context.Loais.ToList();
            return Ok(dsLoai);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var loai = _context.Loais.SingleOrDefault(x => x.MaLoai == id);
            if (loai == null) return NotFound();
            return Ok(loai);
        }

        [HttpPost]
        public IActionResult Create(LoaiVm loaiVm)
        {
            try
            {
                var loai = new Loai
                {
                    TenLoai = loaiVm.TenLoai
                };
                _context.Add(loai);
                _context.SaveChanges();
                return Ok(loai);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public IActionResult GetById(int id, LoaiVm loaiVm)
        {
            var loai = _context.Loais.SingleOrDefault(x => x.MaLoai == id);
            if (loai == null) return NotFound();
            loai.TenLoai = loaiVm.TenLoai;
            _context.SaveChanges();
            return NoContent();
        }
    }
}
