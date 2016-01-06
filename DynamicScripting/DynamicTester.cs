using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Web;
using DynamicScripting.Dynamics;
using DynamicScripting.Pocos;
using Xunit;

namespace DynamicScripting
{
    public class DynamicTester
    {
        [Fact]
        public void Test1()
        {
            Assert.Equal("IMyInterface", this.GetClassNameImplementor(this.MakeInstanceImplA()));

            Assert.Equal("MyInterfaceImplementorA", this.GetClassNameImplementor(this.MakeInstanceImplA() as dynamic));

            Assert.Equal("MyInterfaceImplementorB", this.GetClassNameImplementor(this.MakeInstanceImplB() as dynamic));
        }

        private IMyInterface MakeInstanceImplA()
        {
            return new MyInterfaceImplementorA();
        }

        private IMyInterface MakeInstanceImplB()
        {
            return new MyInterfaceImplementorB();
        }

        private string GetClassNameImplementor(IMyInterface impl)
        {
            return "IMyInterface";
        }

        private string GetClassNameImplementor(MyInterfaceImplementorA impl)
        {
            return "MyInterfaceImplementorA";
        }

        private string GetClassNameImplementor(MyInterfaceImplementorB impl)
        {
            return "MyInterfaceImplementorB";
        }

        [Fact]
        public void TestOnDynamicCollection()
        {
            dynamic cache = new List<Person>
            {
                new Person{ Id = 1, Name = "name1", Surname = "surname1" },
                new Person{ Id = 2, Name = "name2", Surname = "surname2" }
            };
            
            Func<Person, bool> func = person => person.Id > 1;
            var res = Enumerable.Where(cache, func);
            Assert.NotEmpty(res);

        }

        [Fact]
        public void TestOnEncoding()
        {
            //  "%5F" -> "_"
            //  "%2" -> "."
            var str = "_";
            //str = "@";
            str = "http://bk.esprinet.com/intranet2/account/tologon?returnUrl=http://bk.esprinet.com:81/intranet/Espricustomer/index.asp";

            Console.WriteLine(HttpUtility.UrlEncode(str));

            Console.WriteLine(HttpUtility.UrlDecode("%5F"));
            Console.WriteLine(HttpUtility.UrlDecode("%2E"));

            Console.WriteLine(HttpUtility.UrlEncode("_"));
            Console.WriteLine(HttpUtility.UrlEncode("."));
        }

        [Fact]
        public void DecodeUrl()
        {
            var urlEncoded = "https://sso-dev.esprinet.com/formauth/Account/Login?ReturnUrl=%2f%3fwa%3dwsignin1.0%26wtrealm%3dhttp%253a%252f%252fcustomsts%252fMyRelyingParty1%26wctx%3drm%253d0%2526id%253dpassive%2526ru%253d%25252fMyRelyingParty1%25252fuser%26wct%3d2015-11-06T13%253a51%253a24Z%26wreply%3dhttp%253a%252f%252fcustomsts%252fMyRelyingParty1%252f&wa=wsignin1.0&wtrealm=http%3a%2f%2fcustomsts%2fMyRelyingParty1&wctx=rm%3d0%26id%3dpassive%26ru%3d%252fMyRelyingParty1%252fuser&wct=2015-11-06T13%3a51%3a24Z&wreply=http%3a%2f%2fcustomsts%2fMyRelyingParty1%2f";
            Console.WriteLine(HttpUtility.UrlDecode(urlEncoded));

            var uri = new Uri(urlEncoded);
            Assert.NotNull(uri);
            
            // 5623
            Console.WriteLine(Base64Encode("5623"));
            Console.WriteLine(Base64Encode("5623 "));
            Console.WriteLine(Base64Encode("5623  "));

            //Console.WriteLine(Base64Decode(Base64Encode("5623")));
            Console.WriteLine(HttpUtility.UrlEncode("NTYyMw=="));

            Console.WriteLine(Base64Decode("NTYyMw=="));
            Console.WriteLine(Base64Decode("NTYyMyAg"));
        }

        [Fact]
        public void DecodeParams()
        {
            var uri = new Uri("http://www.contoso.com/default.aspx?fullname=Fadi%20Fakhouri");
            Assert.Equal("?fullname=Fadi%20Fakhouri", uri.Query);

            var allParams = uri.Query.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item =>
                {
                    var keyValue = item.Split(new[] { "=" }, StringSplitOptions.None);
                    return new KeyValuePair<string, string>(keyValue[0], keyValue[1]);
                })
                ;
            Assert.True(allParams.Any());
        }

        [Fact]
        public void PingTester()
        {
            var url = new Uri("https://tribe.esprinet.com/");
            using (var ping = new Ping())
            {
                var response = ping.Send(url.Authority);
                Assert.Equal(IPStatus.Success, response.Status);
            }
        }

        /// <summary>
        /// Codifica in Base64 la stringa in input.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes, Base64FormattingOptions.InsertLineBreaks);
        }

        /// <summary>
        /// Decodifica in Base64 la stringa in input.
        /// </summary>
        /// <param name="base64EncodeString">The base64 encoded data.</param>
        /// <returns></returns>
        private static string Base64Decode(string base64EncodeString)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodeString);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
