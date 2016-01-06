using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchTester.Domain
{
    public class Person
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Counter { get; set; }
    }

    public class PersonV2
        : Person
    {
        public int Other { get; set; }
    }
}
