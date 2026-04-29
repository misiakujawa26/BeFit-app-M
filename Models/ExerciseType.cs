using System.ComponentModel.DataAnnotations;

namespace befit.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }
        // public string Name { get; set; }
        [Required(ErrorMessage = "Nazwa ćwiczenia jest wymagana")]
        [StringLength(100, ErrorMessage = "Maksymalnie 100 znaków")]
        public string Name { get; set; }
    }
}
