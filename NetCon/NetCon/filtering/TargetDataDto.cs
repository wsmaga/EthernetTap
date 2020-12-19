using NetCon.util;

namespace NetCon.filtering
{
    public class TargetDataDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public dynamic Value { get; set; }
        public DataType DataType { get; set; } 
        public byte[] RawData { get; set; }
        public bool TriggeredThreshold { get; set; }
        public ThresholdType ThresholdType { get; set; }
        public dynamic ThresholdValue { get; set; }
        public dynamic ThresholdValue2 { get; set; }
        public bool RegisterChanges { get; set; }
    }
}
