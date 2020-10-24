using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ListenerX.Helpers
{
    public static class AssemblyHelpers
    {
        /// <summary>
        /// Load class object from dll file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T LoadInstance<T>(string filePath, params object[] args)
        {
            //var dll = Assembly.LoadFrom(filePath);
            var bytes = File.ReadAllBytes(filePath);
            return LoadInstance<T>(bytes, args);
        }
        /// <summary>
        /// Load class object from byte array data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T LoadInstance<T>(byte[] data, params object[] args)
        {
            var dll = Assembly.Load(data);
            var exportedTypes = dll.GetExportedTypes();
            var mustImplementedByType = typeof(T);
            foreach (var type in exportedTypes)
            {
                bool isAbleToCastToGenericType = mustImplementedByType.IsAssignableFrom(type) || type.FullName == mustImplementedByType.FullName;
                if (isAbleToCastToGenericType)
                {
                    var instance = (T)Activator.CreateInstance(type, args);
                    return instance;
                }
            }
            return default;
        }
        /// <summary>
        /// Load class object from dll file in current domain.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T LoadInstance2<T>(string filePath, params object[] args)
        {
            var assemblyName = Assembly.LoadFrom(filePath).GetName();
            var dll = AppDomain.CurrentDomain.Load(assemblyName);
            var exportedTypes = dll.GetExportedTypes();
            var mustImplementedByType = typeof(T);
            foreach (var type in exportedTypes)
            {
                bool isAbleToCastToGenericType = mustImplementedByType.IsAssignableFrom(type) || type.FullName == mustImplementedByType.FullName;
                if (isAbleToCastToGenericType)
                {
                    var instance = (T)Activator.CreateInstance(type, args);
                    return instance;
                }
            }
            return default;
        }
        /// <summary>
        /// Load class object from dll file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object LoadInstance(string filePath, params object[] args)
        {
            var bytes = File.ReadAllBytes(filePath);
            return LoadInstance(bytes, args);
        }
        /// <summary>
        /// Load class object from byte array data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object LoadInstance(byte[] data, params object[] args)
        {
            var dll = Assembly.Load(data);
            var exportedTypes = dll.GetExportedTypes();
            foreach (var type in exportedTypes)
            {
                var instance = Activator.CreateInstance(type, args);
                return instance;
            }
            return default;
        }
    }
}
