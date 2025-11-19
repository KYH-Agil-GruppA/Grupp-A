using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs
{
    public class UserDto
    {

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;



        public List<SessionsDto> Bookings { get; set; } = [];

        public bool IsDeleted { get; set; }
    }
}
