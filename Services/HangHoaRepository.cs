using FreeCourseApiNet5.Data;
using FreeCourseApiNet5.Models;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourseApiNet5.Services
{
    public class HangHoaRepository : IHangHoaRepository
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 3;

        public HangHoaRepository(MyDbContext context)
        {
            _context = context;
        }
        public List<HangHoaModel> GetAll(string search, double? from, double? to, string sortBy,int page)
        {
            var hanghoas = _context.HangHoas.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                hanghoas = hanghoas.Where(hh => hh.TenHh.Contains(search.Trim()));
            }

            if (from.HasValue)
            {
                hanghoas = hanghoas.Where(hh => hh.DonGia >= from);
            }
            if (to.HasValue)
            {
                hanghoas = hanghoas.Where(hh => hh.DonGia <= to);
            }

            #region Sorting
            //default sort by Name (TenHH)
            hanghoas = hanghoas.OrderBy(hh => hh.TenHh);
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "tenhh_desc": hanghoas = hanghoas.OrderByDescending(hh => hh.TenHh);
                        break;
                    case "gia_asc":
                        hanghoas = hanghoas.OrderBy(hh => hh.DonGia);
                        break;
                    case "gia_desc":
                        hanghoas = hanghoas.OrderByDescending(hh => hh.DonGia);
                        break;
                }
            }
            #endregion

            #region Paging
            hanghoas=hanghoas.Skip((page-1)*PAGE_SIZE).Take(PAGE_SIZE);
            #endregion

            var result = hanghoas.Select(hh => new HangHoaModel
            {
                MaHangHoa = hh.MaHh,
                TenHangHoa = hh.TenHh,
                DonGia = hh.DonGia,
                TenLoai = hh.Loai.TenLoai,
            });
            #endregion

            return result.ToList();
        }
    }
}
