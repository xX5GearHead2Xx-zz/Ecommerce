namespace Ecommerce.Models
{
    public class BankAccountDetails
    {
        public string Bank { get; }
        public string AccountNumber { get; }
        public string AccountType { get; }
        public string BranchCode { get; }
        public string SwiftCode { get; }
        public BankAccountDetails()
        {
            Bank = Ecommerce.Startup.StaticConfiguration["BankAccountDetails:Bank"];
            AccountNumber = Ecommerce.Startup.StaticConfiguration["BankAccountDetails:AccountNumber"];
            AccountType = Ecommerce.Startup.StaticConfiguration["BankAccountDetails:AccountType"];
            BranchCode = Ecommerce.Startup.StaticConfiguration["BankAccountDetails:BranchCode"];
            SwiftCode = Ecommerce.Startup.StaticConfiguration["BankAccountDetails:SwiftCode"];
        }
    }
}
