using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public List<SessionDeleteDto> SessionDeletes { get; set; } = new List<SessionDeleteDto>();

        public bool IsDeleted { get; set; }
    }
}
