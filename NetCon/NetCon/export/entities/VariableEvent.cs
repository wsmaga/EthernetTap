using NetCon.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.export.entities
{
    class VariableEvent
    {
        public long VariableEventID { get; set; }
        public DateTime Date { get; set; }
        public byte[] ThresholdValue { get; set; }
        public byte[] ThresholdValue2 { get; set; }
        public ThresholdType thresholdType { get; set; }
        public enums.DataType thresholdDataType { get; set; }

        [Required]
        public virtual Target Target { get; set; }
    }
}
