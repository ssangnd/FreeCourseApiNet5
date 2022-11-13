using System.ComponentModel.DataAnnotations;

namespace FreeCourseApiNet5.Models
{
    public class LoaiModel
    {
        [Required]
        [MaxLength(50)]
        public string TenLoai { get; set; }
    }
}
