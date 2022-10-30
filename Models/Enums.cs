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

        public enum PaymentOption
        {
            NotSet=-1,
            Paygate,
            Manual
        }

        public enum Province
        {
            Unknown = -1,
            Gauteng,
            CapeTown,
            FreeState,
            EasternCape,
            Limpopo,
            Mpumalanga,
            KwazuluNatal,
            NorthernCape,
            NorthWest,
            WesternCape
        }

        public enum PropertyType
        {
            Unknown = -1,
            Residential,
            Business,
        }
    }
}
