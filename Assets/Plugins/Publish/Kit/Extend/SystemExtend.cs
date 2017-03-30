// using UnityEngine;
using System;
using System.Reflection;
namespace Kit.Extend
{
    public static class SystemExtend
    {
        #region DebugFunctions
        /// <summary>
        /// 	- Gets the methods of an object.
        /// </summary>
        /// <returns>
        /// 	- A list of methods accessible from this object.
        /// </returns>
        /// <param name='obj'>
        /// 	- The object to get the methods of.
        /// </param>
        /// <param name='includeInfo'>
        /// 	- Whether or not to include each method's method info in the list.
        /// </param>
        public static string MethodsOfObject(this Object obj, bool includeInfo = false)
        {
            string methods = string.Empty;
            MethodInfo[] methodInfos = obj.GetType().GetMethods();
            for (int i = 0; i < methodInfos.Length; i++)
            {
                if (includeInfo)
                {
                    methods += methodInfos[i] + "\n";
                }
                else
                {
                    methods += methodInfos[i].Name + "\n";
                }
            }
            return (methods);
        }

        /// <summary>
        /// 	- Gets the methods of a type.
        /// </summary>
        /// <returns>
        /// 	- A list of methods accessible from this type.
        /// </returns>
        /// <param name='type'>
        /// 	- The type to get the methods of.
        /// </param>
        /// <param name='includeInfo'>
        /// 	- Whether or not to include each method's method info in the list.
        /// </param>
        public static string MethodsOfType(this Type type, bool includeInfo = false)
        {
            string methods = string.Empty;
            MethodInfo[] methodInfos = type.GetMethods();
            for (var i = 0; i < methodInfos.Length; i++)
            {
                if (includeInfo)
                {
                    methods += methodInfos[i] + "\n";
                }
                else
                {
                    methods += methodInfos[i].Name + "\n";
                }
            }
           return (methods);
        }
        #endregion
    }
}