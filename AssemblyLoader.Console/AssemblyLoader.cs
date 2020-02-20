﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyLoader.Console
{
    public static class AssemblyLoader
    {
        public static void ResolveAssembly(this AppDomain domain, string assemblyFullName)
        {
            try
            {
                domain.CreateInstance(assemblyFullName, "AnyType");
            }
            catch (Exception)
            {
                System.Console.WriteLine($"Load assemblies: {assemblyFullName}");
            }
        }

        public static void ResolveAssemblies<TResource>(this AppDomain domain, Func<byte[], byte[]> converter = null)
        {
            var assemblies = ResolveAssembliesFromStaticResource<TResource>(converter);
            ResolveAssemblies(domain, assemblies);
        }

        public static void ResolveAssemblies(this AppDomain domain, byte[] assembly)
        {
            var assemblies = ResolveAssembliesFromStaticBytes(assembly);
            ResolveAssemblies(domain, assemblies);
        }

        public static void ResolveAssemblies(this AppDomain domain, List<Assembly> assemblies)
        {
            ResolveEventHandler handler = (sender, args) => assemblies.Find(a => a.FullName == args.Name);
            domain.AssemblyResolve += handler;
            assemblies.ForEach(a => ResolveAssembly(domain, a.FullName));
            domain.AssemblyResolve -= handler;
        }

        public static void ResolveAssembly(this AppDomain domain, Assembly assembly)
        {
            ResolveEventHandler handler = (sender, args) => assembly;
            domain.AssemblyResolve += handler;
            ResolveAssembly(domain, assembly.FullName);
            domain.AssemblyResolve -= handler;
        }

        public static List<Assembly> ResolveAssembliesFromStaticResource<TResource>(Func<byte[], byte[]> converter = null)
        {
            var assemblyDatyType = typeof(byte[]);
            var assemblyDataItems =
                typeof(TResource)
                    .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(p => p.PropertyType == assemblyDatyType)
                    .Select(p => p.GetValue(null, null))
                    .Cast<byte[]>()
                    .ToList();

            var assemblies = new List<Assembly>();
            foreach (var assemblyData in assemblyDataItems)
            {
                try
                {
                    var rawAssembly = converter == null ? assemblyData : converter(assemblyData);
                    var assembly = Assembly.Load(rawAssembly);
                    assemblies.Add(assembly);
                }
                catch (Exception exception)
                {
                    System.Console.WriteLine($"{exception.Message}");
                }
            }

            return assemblies;
        }

        private static List<Assembly> ResolveAssembliesFromStaticBytes(byte[] rawAssembly)
        {
            var assemblies = new List<Assembly>();
            try
            {
                var assembly = Assembly.Load(rawAssembly);
                assemblies.Add(assembly);
            }
            catch (Exception exception)
            {
                System.Console.WriteLine($"{exception.Message}");
            }

            return assemblies;
        }
    }
}
