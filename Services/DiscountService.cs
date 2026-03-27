namespace EasterCrime.Services
{
    public class DiscountService
    {
        public decimal ApplyDiscount(string role, decimal amount)
        {
            // BUG: Role er sjekket med = i stedet for ==
            if (role = "Admin")  
            {
                return amount * 0.5m; // halv pris for Admin
            }

            return amount; // ingen rabatt
        }
    }
}