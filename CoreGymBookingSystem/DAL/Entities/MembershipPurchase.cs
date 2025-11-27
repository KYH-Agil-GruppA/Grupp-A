using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class MembershipPurchase
    {
        public int Id { get; set; }

        public int MembershipTypeId { get; set; }
        public MembershipType MembershipType { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
    }
}

