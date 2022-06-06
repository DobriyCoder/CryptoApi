namespace CryptoApi.Models
{
    public enum EAvailability
    {
        App, 
        Dex, 
        Appdex, 
        Soon, 
        NotAvaliable
    }
    public static class CConst
    {
        public const double NETWORK_FEE = 0.00968022;
        public const EAvailability AVAILABILITY = EAvailability.Soon;
    }
}
