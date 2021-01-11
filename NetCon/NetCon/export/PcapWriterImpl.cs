using NetCon.model;
using System.IO;

namespace NetCon.export
{

    class PcapWriterImpl : IPcapWriter<Frame>
    {
        BinaryWriter writer;

        public bool isInitialized => writer != null;

        public void InitWrite(string fileName)  //TODO sprawdzić czy plik istnieje. Jeśli tak to otworzyć w trybie append. Jeśli nie otworzyć w trybie create.
        {
            writer = new BinaryWriter(File.Open(fileName, FileMode.Create));

            //Write header
            writer.Write(0xA1B23C4D);
            writer.Write(0x00040002);
            writer.Write(0x00000000);
            writer.Write(0x00000000);
            writer.Write(0xFFFFFFFF);
            writer.Write(0x00000112);
        }

        public void WriteFrame(Frame frame)
        {
            if (writer == null)
                throw new WriterNotInitializedException();
            writer.Write(frame.RawData);
        }

        public void EndWrite()
        {
            writer.Close();
            writer = null;
        }
    }
}
