using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.export.entities
{
    class TargetName
    {
        public TargetName()
        {
            Targets = new HashSet<Target>();
        }

        public long TargetNameID { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public virtual ICollection<Target> Targets { get; set; }
    }
}
