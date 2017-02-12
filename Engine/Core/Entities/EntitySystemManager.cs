using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hippopotamus.Engine.Core.Exceptions;
using Ninject;

namespace Hippopotamus.Engine.Core
{
    public static class EntitySystemManager
    {
        private static readonly Dictionary<Type, EntitySystem> systems = new Dictionary<Type, EntitySystem>();

        public static void Initialize()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if(type.BaseType != typeof(EntitySystem)) continue;

                    object[] attribute = type.GetCustomAttributes(typeof(StartupEntitySystem), false);
                    if (attribute.Length > 0)
                    {
                        Register(type);
                    }
                }
            }
        }

        public static void Register(Type type)
        {
            if (type.BaseType != typeof(EntitySystem) || systems.ContainsKey(type)) return;

            EntitySystem system = (EntitySystem) Activator.CreateInstance(type);
            systems.Add(type, system);

            Type[] interfaces = type.GetInterfaces();
            if (interfaces.Contains(typeof(IUpdatable)))
            {
                IUpdatable updatable = system as IUpdatable;
                if (updatable != null)
                {
                    DependencyInjector.Kernel.Get<GameEngine>().GameLoop.Register(updatable.Update);
                }
            }

            if (!interfaces.Contains(typeof(IDrawable))) return;
            IDrawable drawable = system as IDrawable;
            if (drawable != null)
            {
                DependencyInjector.Kernel.Get<GameEngine>().GameLoop.Register(drawable.Draw);
            }
        }

        public static void Register<T>() where T : EntitySystem, new()
        {
            Register(typeof(T));
        }

        public static void Unregister(Type type)
        {
            if (!systems.ContainsKey(type))
            {
                throw new EntitySystemNotFoundException(type);
            }

            EntitySystem system = systems[type];
            Type[] interfaces = type.GetInterfaces();
            if (interfaces.Contains(typeof(IUpdatable)))
            {
                IUpdatable updatable = system as IUpdatable;
                if (updatable != null)
                {
                    DependencyInjector.Kernel.Get<GameEngine>().GameLoop.Unregister(updatable.Update);
                }
            }

            if (!interfaces.Contains(typeof(IDrawable))) return;
            IDrawable drawable = system as IDrawable;
            if (drawable != null)
            {
                DependencyInjector.Kernel.Get<GameEngine>().GameLoop.Unregister(drawable.Draw);
            }

            systems.Remove(type);
        }

        public static void Unregister<T>() where T : EntitySystem
        {
            Unregister(typeof(T));
        }
             
        public static T Get<T>() where T : EntitySystem, new()
        {
            if (!systems.ContainsKey(typeof(T)))
            {
                Register<T>();
            }

            return (T)systems[typeof(T)];
;        }
    }
}