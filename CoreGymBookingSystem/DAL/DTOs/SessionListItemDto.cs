using DAL.Enums;

using System;

namespace DAL.DTOs
{
    public class SessionListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Category Category { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxParticipants { get; set; }
    }
}
