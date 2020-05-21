using NetCon.inter;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetCon.viewmodel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string sampleText { get; set; }

        public int counter { get; set; } = 0;

        public MainWindowViewModel()
        {

            var impl = new NetConImpl();

            Task.Run(async () =>
            {

                impl.sendRequest(RequestCode.BRIDGE_SWITCH, 1, true);
                impl.sendRequest(RequestCode.BRIDGE_SWITCH, 2, true);
                impl.sendRequest(RequestCode.BRIDGE_SWITCH, 3, true);
                impl.sendRequest(RequestCode.BRIDGE_SWITCH, 4, true);

                impl.sendAndReceiveMdio();

                string[] strVec = new string[] { "NetCon.exe", "set", "3", "0" };

                impl.sendSettings(strVec);
            });
        }

        public MainWindowViewModel(MainWindow wnd)
        {
            counter = 0;
            var impl = new NetConImpl();
           
            Task.Run(async () =>
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open("capture_si_szarp.pcap", FileMode.Create)))
                {
                    impl.setOnFrameListener((IntPtr arr, int size) => {
                        byte[] data = new byte[size];
                        Marshal.Copy(arr, data, 0, size);       //Kopiowanie danych. Może być problem z wydajnością w RT !!
                        counter += 1;
                        sampleText = System.Text.Encoding.Default.GetString(data);
                        //sampleText = counter.ToString();

                        //zapis do pliku
                        writer.Write(data);
                        return data.Length; //TODO policzyć ile faktycznie bajtów odebrano i zwrócić tu! 
                    });

                    writer.Write(0xA1B23C4D);
                    writer.Write(0x00040002);
                    writer.Write(0x00000000);
                    writer.Write(0x00000000);
                    writer.Write(0xFFFFFFFF);
                    writer.Write(0x00000112);

                    Task.Run(async () =>impl.startCapture());
                    await Task.Delay(20000);
                    impl.stopCapture();
                    await Task.Delay(5000);
                    writer.Close();
                }     
            });
        }


    }
}
