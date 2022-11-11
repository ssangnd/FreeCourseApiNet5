using FreeCourseApiNet5.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourseApiNet5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HangHoaController : ControllerBase
    {
        public static List<HangHoa> hangHoas = new List<HangHoa>();

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(hangHoas);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            //linq
            var hangHoa = hangHoas.SingleOrDefault(x => x.MaHangHoa == Guid.Parse(id));
            if (hangHoa == null)
            {
                return NotFound();
            }
            return Ok(hangHoa);
        }


        [HttpPost]
        public IActionResult Create(HangHoaVM hangHoaVM)
        {
            var hanghoa = new HangHoa
            {
                MaHangHoa = Guid.NewGuid(),
                TenHangHoa = hangHoaVM.TenHangHoa,
                DonGia = hangHoaVM.DonGia,
            };

            hangHoas.Add(hanghoa);
            return Ok(new
            {
                Success = true,
                Data = hanghoa
            });
        }

        [HttpPut("{id}")]
        public IActionResult Edit(string id, HangHoaVM hangHoaVM)
        {
            try
            {
                var hangHoa = hangHoas.SingleOrDefault(x => x.MaHangHoa == Guid.Parse(id));
                if (hangHoa == null) return NotFound();

                if (id != hangHoa.MaHangHoa.ToString()) return BadRequest();
                hangHoa.TenHangHoa = hangHoaVM.TenHangHoa;
                hangHoa.DonGia = hangHoaVM.DonGia;
                return Ok()
;            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public IActionResult Remove(string id)
        {
            var hangHoa = hangHoas.SingleOrDefault(x => x.MaHangHoa == Guid.Parse(id));
            if (hangHoa == null)
            {
                return NotFound();
            }
            hangHoas.Remove(hangHoa);
            return Ok(hangHoa);
        }
    }
}
