using System;
using System.Collections.Generic;
using System.Reactive;
#nullable enable

namespace MCDSaveEdit
{
    public abstract class ObservableBase<T>: IObservable<T>
    {
        private readonly HashSet<IObserver<T>> _observers = new HashSet<IObserver<T>>();

        protected void sendNext(T nextValue)
        {
            foreach(var observer in _observers)
            {
                observer.OnNext(nextValue);
            }
        }

        public void subscribe(Action<T> onNext)
        {
            this.subscribe(Observer.Create<T>(onNext));
        }

        public virtual void subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
        }
        public virtual void unsubscribe(IObserver<T> observer)
        {
            _observers.Remove(observer);
        }
    }


    public interface IObservable<T>
    {
        void subscribe(Action<T> onNext);
        void subscribe(IObserver<T> observer);
        void unsubscribe(IObserver<T> observer);
    }

    public interface IReadProperty<T> : IObservable<T>
    {
        T value { get; }
    }

    public interface IWriteProperty<T> : IObservable<T>
    {
        T setValue { set; }
    }

    public interface IReadWriteProperty<T> : IReadProperty<T>, IWriteProperty<T>
    {
    }

    public class Property<T> : ObservableBase<T>, IReadWriteProperty<T>
    {
        protected T _value;

        public T value { get { return _value; } set { _value = value; sendNext(value); } }
        public T setValue { set { this.value = value; } }
        public Property(T value)
        {
            _value = value;
        }
    }

    public static class PropertyExtensions
    {
        public static MappedProperty<THidden, TVisible> map<THidden, TVisible>(this Property<THidden> property, Func<THidden, TVisible>? getMethod, Action<THidden, TVisible>? setMethod = null)
        {
            return new MappedProperty<THidden, TVisible>(property, getMethod, setMethod);
        }
    }

    public class MappedProperty<THidden, TVisible> : ObservableBase<TVisible>, IReadWriteProperty<TVisible>
    {
        private readonly Func<THidden, TVisible>? _getMethod;
        private readonly Action<THidden, TVisible>? _setMethod;
        private readonly IReadProperty<THidden> _wrappedProperty;
        public MappedProperty(IReadProperty<THidden> wrappedProperty, Func<THidden, TVisible>? getMethod, Action<THidden, TVisible>? setMethod = null)
        {
            _wrappedProperty = wrappedProperty;
            _getMethod = getMethod;
            _setMethod = setMethod;
        }

        public TVisible value
        {
            get
            {
                return _getMethod!.Invoke(_wrappedProperty.value);
            }
            set
            {
                _setMethod?.Invoke(_wrappedProperty.value, value);
                sendNext(value);
            }
        }
        public TVisible setValue { set { this.value = value; } }


        private readonly Dictionary<IObserver<TVisible>, IObserver<THidden>> _dictionary = new Dictionary<IObserver<TVisible>, IObserver<THidden>>();

        public override void subscribe(IObserver<TVisible> observer)
        {
            base.subscribe(observer);
            var weakObserver = new WeakReference<IObserver<TVisible>>(observer);
            IObserver<THidden> convertedObserver = Observer.Create<THidden>(hiddenValue => {
                if(weakObserver.TryGetTarget(out IObserver<TVisible> target))
                {
                    target?.OnNext(_getMethod!(hiddenValue));
                }
            });
            _dictionary.Add(observer, convertedObserver);
            _wrappedProperty.subscribe(convertedObserver);
        }

        public override void unsubscribe(IObserver<TVisible> observer)
        {
            base.unsubscribe(observer);
            if (_dictionary.TryGetValue(observer, out IObserver<THidden> convertedObserver))
            {
                _dictionary.Remove(observer);
                _wrappedProperty.unsubscribe(convertedObserver);
            }
        }
    }

}
