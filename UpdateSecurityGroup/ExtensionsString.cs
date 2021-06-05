using System;

namespace Microsoft.Toolkit
{
    public static class ExtensionsString
    {
        /// <summary>
        /// Evaluates string to confirm it is a valid IPv4 address.  It does this by:
        ///    1.Confirming 4 octets.
        ///    2.Each octet Is an integer in the range of 0 - 255.
        /// </summary>
        /// <param name="ip">The string to be validated as an IP address</param>
        /// <returns>bool: True if valid IPv4 address; false otherwise</returns>
        public static bool IsIPAddress(this string ip)
        {
            bool isvalid = false;
            string[] octets = ip.Split(".");
            if (octets.Length == 4)
                foreach (string s in octets)
                {
                    if (Int32.TryParse(s, out int oct) &&
                        oct >= 0 && oct <= 255)
                        isvalid = true;
                    else
                        return false;
                } 
            return isvalid;
        }
    }
}
