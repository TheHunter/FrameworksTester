using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ElasticSearchTester.Extensions
{
    public static class StreamEncoder
    {
        /// <summary/>
        /// <param name="dataToEncode"/>
        /// <returns/>
        public static string EncodeTo64(this byte[] dataToEncode)
        {
            return Convert.ToBase64String(dataToEncode);
        }

        /// <summary/>
        /// <param name="dataToEncode"/>
        /// <returns/>
        public static string EncodeTo64(this string dataToEncode)
        {
            return Encoding.ASCII.GetBytes(dataToEncode).EncodeTo64();
        }

        /// <summary/>
        /// <param name="encodedData"/>
        /// <returns/>
        public static byte[] DecodeFrom64(this string encodedData)
        {
            return Convert.FromBase64String(encodedData);
        }

        /// <summary/>
        /// <param name="encodedData"/>
        /// <returns/>
        public static string DecodeFrom64(this byte[] encodedData)
        {
            return Encoding.ASCII.GetString(encodedData);
        }

        /// <summary/>
        /// <param name="str"/>
        /// <returns/>
        public static bool IsBase64String(this string str)
        {
            str = str.Trim();
            if (str.Length % 4 == 0)
                return Regex.IsMatch(str, "^[a-zA-Z0-9\\+/]*={0,3}$", RegexOptions.None);
            
            return false;
        }
    }
}
