using FreeCourseApiNet5.Data;
using FreeCourseApiNet5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            try
            {
                var dsLoai = _context.Loais.ToList();
                return Ok(dsLoai);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var loai = _context.Loais.SingleOrDefault(x => x.MaLoai == id);
            if (loai == null) return NotFound();
            return Ok(loai);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(LoaiModel loaiModel)
        {
            try
            {
                var loai = new Loai
                {
                    TenLoai = loaiModel.TenLoai
                };
                _context.Add(loai);
                _context.SaveChanges();
                //return Ok(loai);
                return StatusCode(StatusCodes.Status201Created, loai);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public IActionResult GetById(int id, LoaiModel loaiModel)
        {
            var loai = _context.Loais.SingleOrDefault(x => x.MaLoai == id);
            if (loai == null) return NotFound();
            loai.TenLoai = loaiModel.TenLoai;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteLoaiById(int id)
        {
            var loai = _context.Loais.SingleOrDefault(lo => lo.MaLoai == id);
            if (loai != null)
            {
                _context.Loais.Remove(loai);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
