using System;
using System.Collections.Generic;

namespace NetCon.util
{
    public class Subject<T> : IObservable<T>
    {
        List<IObserver<T>> observers = new List<IObserver<T>>();
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> _observers;
            private IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                _observer = observer;
                _observers = observers;
            }
            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void pushNextValue(T obj)
        {
            foreach(var observer in observers)
            {
                if (obj == null)
                {
                    observer.OnError(new NullReferenceException());
                }
                else
                    observer.OnNext(obj);
            }
        }

        public void complete()
        {
            foreach(var observer in observers.ToArray())
            {
                if (observers.Contains(observer))
                    observer.OnCompleted();
            }

            observers.Clear();
        }
    }
}
