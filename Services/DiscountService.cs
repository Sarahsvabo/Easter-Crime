namespace EasterCrime.Services
{
    public class DiscountService
    {
        public decimal ApplyDiscount(string role, decimal amount)
        {
          
            if (role = "Admin")  
            {
                return amount * 0.5m; 
            }

            return amount;
        }
    }
}