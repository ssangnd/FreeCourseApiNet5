using FreeCourseApiNet5.Models;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourseApiNet5.Services
{
    public class LoaiRepositoryInMemory : ILoaiRepository
    {

        static List<LoaiVm> loais = new List<LoaiVm>
        {
            new LoaiVm{MaLoai=1, TenLoai="Tivi"},
            new LoaiVm{MaLoai=2, TenLoai="Tu Lanh"},
            new LoaiVm{MaLoai=3, TenLoai="Dieu Hoa"},
            new LoaiVm{MaLoai=4, TenLoai="May Giat"},
        };

        public LoaiVm Add(LoaiModel loai)
        {
            var _loai = new LoaiVm
            {
                MaLoai = loais.Max(lo=>lo.MaLoai)+1,
                TenLoai = loai.TenLoai,
            };
            loais.Add(_loai);
            return _loai;
        }

        public void Delete(int id)
        {
            var _loai = loais.SingleOrDefault(lo => lo.MaLoai == id);
            loais.Remove(_loai);
        }

        public List<LoaiVm> GetAll()
        {
            return loais;
        }

        public LoaiVm GetById(int id)
        {
            return loais.SingleOrDefault(x=>x.MaLoai==id);
        }

        public void Update(LoaiVm loai)
        {
            var _loai= loais.SingleOrDefault(x => x.MaLoai == loai.MaLoai);
            if(loai!=null)
            {
                _loai.TenLoai = loai.TenLoai;
            } 
        }
    }
}
