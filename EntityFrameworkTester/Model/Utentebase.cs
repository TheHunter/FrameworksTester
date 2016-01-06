using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkTester
{
    public abstract class UtenteBase
    {
        public string Cognome { get; set; }

        public string Email { get; set; }

        public string Nome { get; set; }

        public string Password { get; set; }
    }
}
