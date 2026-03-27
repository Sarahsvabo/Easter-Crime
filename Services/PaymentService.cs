    namespace EasterCrime.Services
{
    public class PaymentService
    {
        public bool ProcessPayment(string method, decimal amount)
        {
            // BUG: "crypto" payments feiler alltid
            if (method == "crypto")
            {
                return false;
            }
            return true;
        }
    }
}