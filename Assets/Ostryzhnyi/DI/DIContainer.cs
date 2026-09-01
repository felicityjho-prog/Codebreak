
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ostryzhnyi.DI.Interfaces;
using UnityEngine;

namespace Ostryzhnyi.DI
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InjectAttribute : Attribute { }

    public class DIContainer
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public void Register<TService>(TService implementation)
        {
            var type = typeof(TService);
            
            if (!_services.TryAdd(type, implementation))
            {
                throw new InvalidOperationException($"{type.Name} is already registered.");
            }
            
            InjectDependencies(type);

            if (implementation is IInitializable initializable)
            {
                initializable.Initialize();
            }
        }
        public void Register<TService>() where TService : new()
        {
            var type = typeof(TService);
            var instance = new TService();
            
            if (!_services.TryAdd(type, instance))
            {
                throw new InvalidOperationException($"{type.Name} is already registered.");
            }
            
            InjectDependencies(instance);

            if (instance is IInitializable initializable)
            {
                initializable.Initialize();
            }
        }

        public void Register<TService>(Func<TService> factory)
        {
            var type = typeof(TService);

            if (_services.ContainsKey(type))
            {
                throw new InvalidOperationException($"{type.Name} is already registered.");
            }
            InjectDependencies(type);

            _services[type] = factory();
        }

        public void RegisterSignal<TSignal>() where TSignal : new()
        {
            var type = typeof(TSignal);
            if (!_services.TryAdd(type, new TSignal()))
            {
                throw new InvalidOperationException($"{type.Name} is already registered.");
            }
        }

        public TService Resolve<TService>()
        {
            var type = typeof(TService);
            if (!_services.TryGetValue(type, out var service))
            {
                throw new InvalidOperationException($"{type.Name} is not registered.");
            }
            return (TService)service;
        }

        public object Resolve(Type type)
        {
            if (!_services.TryGetValue(type, out var service))
            {
                throw new InvalidOperationException($"{type.Name} is not registered.");
            }
            return service;
        }

        public bool TryResolve(Type type, out object dependency)
        {
            if (_services.TryGetValue(type, out var service))
            {
                dependency = service;
                return true;
            }

            dependency = default;
            return false;
        }

        public void InjectDependencies(object target)
        {
            var targetType = target.GetType();

            foreach (var field in GetAllFields(targetType)
                         .Where(f => f.IsDefined(typeof(InjectAttribute), false)))
            {
                if (TryResolve(field.FieldType, out var dependency))
                {
                    field.SetValue(target, dependency);
                }
            }
        }

        public void InjectGameObject(GameObject targetGameObject)
        {
            foreach (var target in targetGameObject.GetComponentsInChildren<MonoBehaviour>())
            {
                InjectDependencies(target);
            }
        }
        
        private IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            if (type == null) return Enumerable.Empty<FieldInfo>();
            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Concat(GetAllFields(type.BaseType));
        }
    }
}
