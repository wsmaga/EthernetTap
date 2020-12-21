using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCon.export.entities
{
    class TargetName
    {
        public TargetName()
        {
            Targets = new HashSet<Target>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TargetNameID { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public virtual ICollection<Target> Targets { get; set; }
    }
}
