using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNet;

public class ServiceRepository
{
    private static readonly Dictionary<Type, Tuple<Type, Func<object>, ServiceType>> repository = new();
    private static readonly Dictionary<Type, object> singletonRepository = new();

    public static void AddScoped<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        if (typeof(TImplementation).GetConstructors(BindingFlags.Public).Count() > 1)
        {
            throw new NotSupportedException($"{typeof(TImplementation)} should have at least one public constructor.");
        }

        repository[typeof(TService)] = new Tuple<Type, Func<object>, ServiceType>(typeof(TImplementation), null, ServiceType.Scoped);
    }

    public static void AddScoped<TService>(Func<TService> options)
        where TService : class
    {
        repository[typeof(TService)] = new Tuple<Type, Func<object>, ServiceType>(null, options, ServiceType.Scoped);
    }

    public static void AddSingleton<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        if (typeof(TImplementation).GetConstructors(BindingFlags.Public).Count() > 1)
        {
            throw new NotSupportedException($"{typeof(TImplementation)} should have at least one public constructor.");
        }

        repository[typeof(TService)] = new Tuple<Type, Func<object>, ServiceType>(typeof(TImplementation), null, ServiceType.Singleton);
    }

    public static void AddSingleton<TService>(Func<TService> builder)
        where TService : class
    {
        repository[typeof(TService)] = new Tuple<Type, Func<object>, ServiceType>(null, builder, ServiceType.Singleton);
    }

    public static TService GetService<TService>()
        where TService : class
    {
        return (TService)GetService(typeof(TService));
    }

    private static object GetService(Type _type)
    {
        var implementationInfo = repository.GetValueOrDefault(_type);

        if (implementationInfo.Item3 == ServiceType.Singleton && singletonRepository.TryGetValue(_type, out object value))
        {
            return value;
        }

        object instance = null;

        if (implementationInfo.Item2 is not null)
        {
            instance = implementationInfo.Item2?.Invoke();
        }
        else if (implementationInfo.Item1 is not null)
        {
            instance = CreateInstance(implementationInfo.Item1);
        }

        if (instance is null)
        {
            throw new NotSupportedException($"There is no implementation for {_type}.");
        }

        if (implementationInfo.Item3 == ServiceType.Singleton)
        {
            singletonRepository.TryAdd(_type, instance);
        }

        return instance;
    }

    private static object CreateInstance(Type _type)
    {
        var cnstructor = _type.GetConstructors().First();

        if (cnstructor.GetParameters().Any())
        {
            var args = new List<object>();

            foreach (var item in cnstructor.GetParameters())
            {
                args.Add(GetService(item.ParameterType));
            }

            return Activator.CreateInstance(_type, args.ToArray());
        }
        else
        {
            return Activator.CreateInstance(_type);
        }
    }

    private enum ServiceType
    {
        Scoped,
        Singleton
    }
}