namespace EasterCrime.Models
{
    public class User
    {
        public string Role { get; set; } = "User";
        public decimal Amount { get; set; }
        public string Method { get; set; } = "CreditCard";
    }
}