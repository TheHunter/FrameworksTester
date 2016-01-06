using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElasticSearchTester.Domain
{
    public class GenUser<TKey>
    {
        public TKey Id { get; set; }

        public string Nominative { get; set; }
    }
}
