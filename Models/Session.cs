using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace befit.Models
{
    public class Session
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Data rozpoczęcia jest wymagana")]
        public DateTime Start { get; set; }
        [Required(ErrorMessage = "Data zakończenia jest wymagana")]
        public DateTime End { get; set; }
    
        public string? UserId { get; set; }
        public IdentityUser? User  { get; set; }
    }
}
