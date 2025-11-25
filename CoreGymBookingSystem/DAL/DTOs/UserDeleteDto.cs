using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs
{
    public class UserDeleteDto
    {
        public int Id { get; set; }

        public List<SessionDeleteDto> SessionDeletes { get; set; } = new List<SessionDeleteDto>();
   
        public bool IsDeleted { get; set; }
    }
}
