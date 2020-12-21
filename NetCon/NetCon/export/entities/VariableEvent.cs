using NetCon.util;
using System;
using System.ComponentModel.DataAnnotations;

namespace NetCon.export.entities
{
    class VariableEvent
    {
        public long VariableEventID { get; set; }
        public byte[] ThresholdValue { get; set; }
        public byte[] ThresholdValue2 { get; set; }
        public ThresholdType thresholdType { get; set; }
        public util.DataType thresholdDataType { get; set; }

        [Required]
        public virtual Target Target { get; set; }
    }
}
