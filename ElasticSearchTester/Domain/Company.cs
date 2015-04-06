using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchTester.Domain
{
    public class Company
    {
        public Company()
        {
            //this.Employers = new List<Employer>();
        }

        public int? Id { get; set; }

        public string Name { get; set; }

        public List<Employer> Employers { get; set; } 
    }
}
