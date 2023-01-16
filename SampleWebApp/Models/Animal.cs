using System.ComponentModel.DataAnnotations;

namespace SampleWebApp.Models
{
    public class Animal
    {
        [Required(ErrorMessage ="Nazwa jest wymagana")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Opis jest wymagany")]
        public string Description { get; set; }
    }
}