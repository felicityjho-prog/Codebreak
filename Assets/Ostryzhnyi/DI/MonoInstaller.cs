using UnityEngine;

namespace Ostryzhnyi.DI
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        protected DIContainer Container;
        
        protected virtual void Awake()
        {
            InternalRegister();
            Register();
            ResolveObjects();
        }

        private void ResolveObjects()
        {
            var monoBehaviours = FindObjectsOfType<MonoBehaviour>(true);

            foreach (var monoBehaviour in monoBehaviours)
            {
                Container.InjectDependencies(monoBehaviour);
            }
        }

        protected abstract void Register();
        
        private void InternalRegister()
        {
            Container = new DIContainer();
            
            Container.Register(Container);
        }
    }
}