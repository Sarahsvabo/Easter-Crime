    namespace EasterCrime.Services
{
    public class PaymentService
    {
        public bool ProcessPayment(string method, decimal amount)
        {
            
            if (method == "crypto")
            {
                return false;
            }
            return true;
        }
    }
}