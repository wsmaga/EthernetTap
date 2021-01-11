namespace NetCon.model
{
    public class Frame
    {
        //public DataType usefulDataType = DataType.NONE;
        public Frame(byte[] data)
        {
            rawData = data;
        }
        //public byte[] usefulData { get; set; }
        //nie usuwać tego
        private byte[] rawData;
        public byte[] RawData { get { return rawData; } }
    }
}
