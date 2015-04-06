using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchTester.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class Student
    {
        public Student()
        {
            this.Version = 1;
        }

        public int? Id { get; set; }

        public string Name { get; set; }


        public long Size { get; set; }


        public string DataEncoded { get; set; }


        public long? Version { get; private set; }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() == this.GetType())
                return obj.GetHashCode() == this.GetHashCode();

            return false;
        }

        public override int GetHashCode()
        {
            return (this.Name.GetHashCode() - this.DataEncoded.GetHashCode()) * 7;
        }

        
    }
}
