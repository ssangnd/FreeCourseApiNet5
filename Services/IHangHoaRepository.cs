using FreeCourseApiNet5.Models;
using System.Collections.Generic;

namespace FreeCourseApiNet5.Services
{
    public interface IHangHoaRepository
    {
        List<HangHoaModel> GetAll(string search, double? from, double? to, string sortBy, int page);
    }
}
