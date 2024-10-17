using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using SeminarHub.Data;
using static SeminarHub.ValidationConstants.Constants;


namespace SeminarHub.Models
{
    public class SeminarViewModel
    {

        [Required]
        [StringLength(TopicMaxLength, MinimumLength = TopicMinLength)]
        public string Topic { get; set; } = string.Empty;

        [Required]
        [StringLength(LecturerMaxLength, MinimumLength = LecturerMinLength)]

        public string Lecturer { get; set; } = string.Empty;

        [Required]
        [StringLength(DetailsMaxLength, MinimumLength = DetailsMinLength)]
        public string Details { get; set; } = string.Empty;

        [Required] 
        public string DateAndTime { get; set; } = DateTime.Today.ToString(DateAndTimeFormat);

        [Range(DurationMinValue, DurationMaxValue)]
        public int? Duration { get; set; }

        [Required]
        public int CategoryId { get; set; }


        public List<Category> Categories { get; set; } = new List<Category>();

    }
}
