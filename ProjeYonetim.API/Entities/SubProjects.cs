using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeYonetim.API.Entities
{
    public class SubProjects : BaseEntity
    {
        public string Title { get; set; }
        public int EffortClock { get; set; }
        [ForeignKey("UsersId")]
        public int UsersId { get; set; }
        public Users Users { get; set; }
        public int ProjectsId { get; set; }
        public Projects Projects { get; set; }
    }
}
