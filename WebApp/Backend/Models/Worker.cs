using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Backend.Models
{
    public class Worker
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Табель")]
        public string Code => Id.ToString("D4");
        [Display(Name = "ФИО")]
        public required string Workername { get; set; }
        [Display(Name = "Пол")]
        public required bool Sex { get; set; } = true;

        [Display(Name = "Дата приема")]
        [DataType(DataType.Date)]
        public DateOnly HireDate { get; set; }
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }
        [Display(Name = "Дата увольнения")]
        [DataType(DataType.Date)]
        public DateOnly? FireDate { get; set; }
        [Display(Name = "Дата перевода")]
        [DataType(DataType.Date)]
        public DateOnly? MoveDate { get; set; }

        [Display(Name = "Подразделение")]
        [ForeignKey("Subdivision")]
        public int? SubdivisionId { get; set; }
        public Subdivision? Subdivision { get; set; }
        [Display(Name = "Должность")]
        public string? Role { get; set; }
        [Display(Name = "Номер телефона")]
        public required string PhoneNumber { get; set; }
        [Display(Name = "Почта")]
        public string? Email { get; set; }
        public string? PicturePath { get; set; }
        //Свойство для проверки увольнения
        public bool IsFired { get; set; } = false;
    }
}
