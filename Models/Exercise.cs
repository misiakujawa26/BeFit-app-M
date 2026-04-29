using System.ComponentModel.DataAnnotations;

namespace befit.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        [Display(Name = "Lp:")]
        [Required]

        [Range(1, 500, ErrorMessage = "Waga musi być od 1 do 500")]
        public int Weight { get; set; }
        [Display(Name = "Waga")]
        [Required]
        [Range(1, 100)]
        public int NumOfSeries { get; set; }
        [Display(Name = "Numer Serii")]
        [Required]
        [Range(1, 100)]
        public int NumOfReps { get; set; }
        [Display(Name = "Numer powtorzenia")]
        [Required]
        public int ExerciseTypeId { get; set; }
        
         public virtual ExerciseType? ExerciseType { get; set; }
        [Display(Name = "Typ cwiczenia")]
        [Required]
        public int SessionId { get; set; }
        public virtual Session? Session { get; set; }
        

    }
}
