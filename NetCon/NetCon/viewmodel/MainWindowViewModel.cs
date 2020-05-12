using NetCon.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetCon.viewmodel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand _startCountingCommand;
        public ICommand StartCountingCommand
        {
            get
            {
                return _startCountingCommand ?? (_startCountingCommand = new CommandHandler(() => startCounting(), () =>{ return !mCounting; }));
            }
        }

        private ICommand _stopCountingCommand;
        public ICommand StopCountingCommand 
        { 
            get
            {
                return _stopCountingCommand ?? (_stopCountingCommand = new CommandHandler(() => stopCounting(), () => { return mCounting; }));
            } 
        }


        public string sampleText { get; set; } = "Hello!";
        private bool mCounting = false;

        public MainWindowViewModel()
        {
        }

        public void startCounting()
        {
            int counter = 0;
            mCounting = true;
            Task.Run(async ()=>
            {
                while (mCounting)
                {
                    sampleText = counter.ToString();
                    counter++;
                    await Task.Delay(100);
                }
            });
        }

        public void stopCounting()
        {
            mCounting = false;
            sampleText = "Finished!";
        }
    }
}
