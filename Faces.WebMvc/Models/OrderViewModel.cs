namespace Faces.WebMvc.Models
{
    using System.ComponentModel.DataAnnotations;

    public class OrderViewModel
    {
        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Display(Name = "Image File")]
        public IFormFile File { get; set; }
    }
}