using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Backend.Models
{
    public class Subdivision
    {
        public int Id { get; set; }
        [Display(Name = "Код")]
        public string Code => Id.ToString("D4");
        [Display(Name = "Полное наименование")]
        public required string Fullname { get; set; }
        [Display(Name = "Аббревиатура")]
        public required string Shortname { get; set; }
        [Display(Name = "Дата создания")]
        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }
        [Display(Name = "Дата ликвидации")]
        [DataType(DataType.Date)]
        public DateOnly? EndDate { get; set; }
        // Свойство для проверки ликвидации
        public bool IsLiquidated { get; set; } = false;
    }
}
