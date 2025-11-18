using System;
using System.ComponentModel.DataAnnotations;
using DAL.Enums; // your Category enum: Yoga, Running, Weightloss, Cardio, Bodybuilding, Nutrition

namespace DAL.DTOs
{
    public class SessionCreateDto
    {
        [Required, StringLength(80)]
        public string Title { get; set; }

        [StringLength(400)]
        public string Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Range(1, 200)]
        public int MaxParticipants { get; set; }

        [Required]
        public Category Category { get; set; }

        // IdentityUser<int> → int key
        [Required]
        public int InstructorId { get; set; }
    }
}
