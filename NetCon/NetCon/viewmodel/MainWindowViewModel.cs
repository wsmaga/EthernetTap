using NetCon.inter;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetCon.viewmodel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string sampleText { get; set; }
        private bool mCounting = false;

        public MainWindowViewModel()
        {
            var impl = new NetConImpl();
            impl.setOnFrameListener((IntPtr arr, int size) => {
                byte[] data = new byte[size];
                Marshal.Copy(arr, data, 0, size);       //Kopiowanie danych. Może być problem z wydajnością w RT !!
                sampleText += System.Text.Encoding.Default.GetString(data);
                return data.Length; //TODO policzyć ile faktycznie bajtów odebrano i zwrócić tu! 
            });

            Task.Run(async () =>
            {
                impl.startCapture();
                await Task.Delay(10000);
                impl.stopCapture();
            });
        }


    }
}
