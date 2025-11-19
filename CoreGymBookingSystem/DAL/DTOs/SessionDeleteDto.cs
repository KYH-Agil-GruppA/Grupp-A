using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs
{
    public class SessionDeleteDto
    {
        public int Id { get; set; }

        public List<UserDeleteDto> UserDeletes { get; set; } = new List<UserDeleteDto>();

        public bool IsDeleted { get; set; }
    }
}
