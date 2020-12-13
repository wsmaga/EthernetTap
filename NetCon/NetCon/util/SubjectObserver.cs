using System;

namespace NetCon.util
{
    public class SubjectObserver<T> : IObserver<T>
    {
        private Action<T> _onNext;
        private Action<Exception> _onError;
        private Action _onComplete;
        public SubjectObserver(Action<T> onNext)
        {
            _onNext = onNext;
        }

        public SubjectObserver(Action<T> onNext, Action<Exception> onError)
        {
            _onNext = onNext;
            _onError = onError;
        }

        public SubjectObserver(Action<T> onNext, Action<Exception> onError, Action onComplete)
        {
            _onNext = onNext;
            _onError = onError;
            _onComplete = onComplete;
        }

        private IDisposable unsubscriber;

        public virtual void Subscribe(Subject<T> subject)
        {
            if (subject != null)
            {
                unsubscriber = subject.Subscribe(this);
            }
        }

        public void OnCompleted()
        {
            if(_onComplete!=null)
                _onComplete();
        }

        public void OnError(Exception error)
        {
            if(_onError!=null)
                _onError(error);
        }

        public void OnNext(T value)
        {
            _onNext(value);
        }
    }
}

