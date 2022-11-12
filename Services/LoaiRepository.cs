using FreeCourseApiNet5.Data;
using FreeCourseApiNet5.Models;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourseApiNet5.Services
{
    public class LoaiRepository : ILoaiRepository
    {
        private readonly MyDbContext _context;

        public LoaiRepository(MyDbContext context)
        {
            _context = context;
        }
        public LoaiVm Add(LoaiModel loai)
        {
            var _loai = new Loai
            {
                TenLoai = loai.TenLoai
            };
            _context.Add(_loai);
            _context.SaveChanges();
            return new LoaiVm
            {
                MaLoai = _loai.MaLoai,
                TenLoai = _loai.TenLoai,
            };
        }

        public void Delete(int id)
        {
            var loai = _context.Loais.SingleOrDefault(lo => lo.MaLoai == id);
            if(loai!=null)
            {
                _context.Remove(loai);
                _context.SaveChanges();
            }
        }

        public List<LoaiVm> GetAll()
        {
            var loais = _context.Loais.Select(lo => new LoaiVm
            {
                MaLoai=lo.MaLoai,
                TenLoai=lo.TenLoai,
            });
            return loais.ToList();
        }

        public LoaiVm GetById(int id)
        {
            var loai = _context.Loais.SingleOrDefault(x=>x.MaLoai==id);
            if (loai != null) return new LoaiVm { 
                    MaLoai=loai.MaLoai,
                    TenLoai=loai.TenLoai
            };
            return null;
        }

        public void Update(LoaiVm loai)
        {
            var _loai = _context.Loais.SingleOrDefault(lo => lo.MaLoai == loai.MaLoai);
            if (loai != null)
            {
                loai.TenLoai = loai.TenLoai;
                _context.SaveChanges();
            }
        }
    }
}
