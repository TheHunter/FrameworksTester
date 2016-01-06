using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkTester.Mappings
{
    public class UtenteBaseMap<T> : EntityTypeConfiguration<T>
        where T : UtenteBase
    {
    }
}
