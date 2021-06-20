using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeYonetim.API.Entities
{
    public class Users : BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
