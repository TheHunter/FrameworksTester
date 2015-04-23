using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Extensions;

namespace DynamicScripting
{
    public class RegexTest
    {
        [Theory]
        //[InlineData("this is my string token {name0}{name1}-{name2}", 3, @"(\{.*\})+")]             //ko
        //[InlineData("this is my string token {name0}{name1}-{name2}", 3, @"(\{(.*)\})+")]           //ko
        //[InlineData("this is my string token {name0}{name1}-{name2}", 3, @"(\{(.*?)\})+")]         //+- (due token adiacenti non sono presi separatamente)
        [InlineData("this is my string token {name0}{name1}-{name2}", 3, @"(\{[^\}]{1,}\})")]         //+- (due token adiacenti non sono presi separatamente)
        [InlineData("{name0}this is my string token {name1}-{name2}", 3, @"(\{[^\}]{1,}\})")]         //+- (due token adiacenti non sono presi separatamente)
        [InlineData("{name0}this is my string token {name1 }-{ name2}", 3, @"(\{[^\}]{1,}\})")]         //+- (due token adiacenti non sono presi separatamente)
        private void TokenTest(string input, int numTokens, string pattern)
        {
            ArrayList aResults = new ArrayList();
            foreach (Match match in Regex.Matches(input, pattern))
            {
                aResults.Add(match.Value);
                Console.WriteLine(match.Value);
            }
            Assert.Equal(numTokens, aResults.Count);
        }

        [Theory]
        [InlineData("Lorem ipsum {token:me} lala this {token:other} other stuff", 2)]
        private void TokenTest2(string input, int numTokens)
        {
            const string pattern = @"(\{token:(.*?)\})+";
            ArrayList aResults = new ArrayList();
            foreach (Match match in Regex.Matches(input, pattern))
            {
                aResults.Add(match.Value);
            }
            Assert.Equal(numTokens, aResults.Count);
            // It should contain me and other
        }

        [Theory]
        [InlineData("{1},{2},{3}", "{1},{2},{3}", true)]
        [InlineData("{a},{b},{c}", "{c},{a},{B}", false)]
        [InlineData("{abc},{bcd},{cde}", "{cde},{abc},{bcd}", true)]
        [InlineData("{abc},{bcd},{cde}", "{cde},{abc},{Bcd}", false)]
        private void SequenzeCompareTest(string col1, string col2, bool result)
        {
            var sep = new List<string> {","}.ToArray();
            IEnumerable<string> c1 = col1.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> c2 = col2.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            //Assert.Equal(result, c1.SequenceEqual(c2));
            //Assert.Equal(result, c2.SequenceEqual(c1));
            Assert.Equal(result, !c1.Except(c2, StringComparer.Ordinal).Union(c2.Except(c1, StringComparer.Ordinal), StringComparer.Ordinal).Any());

        }
        
        [Fact]
        private void ExampleOnEliminateToken()
        {
            var buffer = new StringBuilder();
            buffer.AppendLine("{");
            buffer.AppendLine("\"$type\": \"Nest.SearchDescriptor`1[[Nest.Tests.Integration.Search.Student, Nest.Tests.Integration]]\",");
            buffer.AppendLine("\"from\": 0,");
            buffer.AppendLine("\"size\": 2,");
            buffer.AppendLine("}");

            var str = buffer.ToString();

            Assert.True(true);
        }
    }
}
