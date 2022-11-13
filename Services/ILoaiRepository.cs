using FreeCourseApiNet5.Models;
using System.Collections.Generic;

namespace FreeCourseApiNet5.Services
{
    public interface ILoaiRepository
    {
        List<LoaiVm> GetAll();
        LoaiVm GetById(int id);
        LoaiVm Add(LoaiModel loai);
        void Update(LoaiVm loai);
        void Delete(int id);
    }
}
