using System.ComponentModel.DataAnnotations;

namespace FreeCourseApiNet5.Models
{
    public class LoaiVm
    {
        [Required]
        [MaxLength(50)]
        public string TenLoai { get; set; }
    }
}
