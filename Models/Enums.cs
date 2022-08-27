using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Models
{
    public class Enums
    {
        public enum AccountSection
        {
            NotSet = -1,
            Orders,
            Reviews,
            Addresses,
            Wishlists,
            PersonalDetails
        }

        public enum ContactType
        {
            NotSet = -1,
            Mobile,
            Home,
            Work,
            Email,
            Other
        }

        public enum OrderStatus
        {
            NotSet = -1,
            Unfinalised,
            Payed,
            Shipped,
            Cancelled
        }

        public enum ClientRole
        {
            NotSet = -1,
            Admin,
            Shopper
        }

        public enum ReviewStatus
        {
            NotSet = -1,
            Pending,
            Approved,
            Rejected
        }
    }
}
