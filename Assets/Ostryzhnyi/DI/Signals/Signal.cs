using System;
using System.Collections.Generic;

namespace Ostryzhnyi.DI.Signals
{
    public class Signal
    {
        private readonly List<Action> _subscribes = new List<Action>();
        
        public void Fire()
        {
            foreach (var subscriber in _subscribes)
            {
                subscriber?.Invoke();
            }
        }

        public void Subscribe(Action action)
        {
            _subscribes.Add(action);
        }

        public void Unsubscribe(Action action)
        {
            _subscribes.Remove(action);
        }
    }
    
    public class Signal<TData> where TData : struct
    {
        private readonly List<Action<TData>> _subscribes = new List<Action<TData>>();

        public void Fire(TData data)
        {
            foreach (var subscriber in _subscribes)
            {
                subscriber?.Invoke(data);
            }
        }

        public void Subscribe(Action<TData> action)
        {
            _subscribes.Add(action);
        }

        public void Unsubscribe(Action<TData> action)
        {
            _subscribes.Remove(action);
        }
    }
    
    public class Signal<TData, TData2> where TData : struct where TData2 : struct
    {
        private readonly List<Action<TData, TData2>> _subscribes = new List<Action<TData, TData2>>();

        public void Fire(TData data, TData2 data2)
        {
            foreach (var subscriber in _subscribes)
            {
                subscriber?.Invoke(data, data2);
            }
        }

        public void Subscribe(Action<TData, TData2> action)
        {
            _subscribes.Add(action);
        }

        public void Unsubscribe(Action<TData, TData2> action)
        {
            _subscribes.Remove(action);
        }
    }
}