using System;

namespace Epinova.NetsPaymentGateway
{
    internal static class AmountHelper
    {
        public static decimal Deflate(int amount)
        {
            return amount / 100m;
        }

        public static string Inflate(decimal amount)
        {
            return Convert.ToString((long)Math.Round(amount * 100, 0, MidpointRounding.AwayFromZero));
        }
    }
}
