using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeYonetim.API.Entities
{
    public class Projects : BaseEntity
    {
        public string Title { get; set; }
        public int EffortClock { get; set; }
        public int UsersId { get; set; }
        public Users Users { get; set; }
    }
}

