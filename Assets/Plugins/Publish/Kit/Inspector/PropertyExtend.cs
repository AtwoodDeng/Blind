#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace Kit.Inspector
{
	/// <summary>For Property Drawer</summary>
	/// <see cref="https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs"/>
	public static class PropertyExtend
	{
		public static object GetCurrent(SerializedProperty prop)
		{
			int num;
			string str;
			object obj = GetObjectLevel(prop, 0, out num, out str);
			// if(num >= 0 && !string.IsNullOrEmpty(str)){}
			return obj;

			//SerializedObject serialObj = new SerializedObject((UnityEngine.Object)obj);
			//SerializedProperty serialProp = serialObj.FindProperty(str);
			//return null;
		}

		public static object GetParent(SerializedProperty prop)
		{
			int i;
			string n;
			return GetObjectLevel(prop, 1, out i, out n);
		}

		public static bool IsArray(SerializedProperty prop)
		{
			return prop.propertyPath.IndexOf(".Array.data[") >= 0;
		}

		public static string LastElementName(SerializedProperty prop)
		{
			string path = prop.propertyPath.Replace(".Array.data[", ".");
			path = path.Replace("]", ".");
			string[] arr = path.Split(new char[] { '.' });
			int pt = arr.Length, tmp;
			while(pt--> 0)
			{
				if(!string.IsNullOrEmpty(arr[pt]) && !int.TryParse(arr[pt], out tmp))
				{
					return arr[pt];
				}
			}
			return null;
		}

		public static int LastElementIndex(SerializedProperty prop)
		{
			int
				start = prop.propertyPath.IndexOf("["),
				end = prop.propertyPath.IndexOf("]");

			if (start < 0)
				return -1;

			start++; // not include first letter

			string numStr = prop.propertyPath.Substring(start, end - start);
			return int.Parse(numStr);
		}

		private static object GetObjectLevel(SerializedProperty prop, int level, out int lastIndex, out string lastName)
		{
			string path = prop.propertyPath.Replace(".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			string[] elements = path.Split('.');
			lastName = null;
			lastIndex = -1;
			foreach (string element in elements.Take(elements.Length - level))
			{
				if (element.Contains("["))
				{
					string elementName = element.Substring(0, element.IndexOf("["));
					int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
					obj = GetValue(obj, elementName, index);
					lastIndex = index;
					lastName = elementName;
				}
				else
				{
					obj = GetValue(obj, element);
					lastName = element;
					lastIndex = -1;
				}
			}
			return obj;
		}

		public static object GetValue(object source, string name)
		{
			if (source == null)
				return null;
			Type type = source.GetType();
			FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (f == null)
			{
				var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (p == null)
					return null;
				return p.GetValue(source, null);
			}
			return f.GetValue(source);
		}

		public static object GetValue(object source, string name, int index)
		{
			IEnumerable enumerable = GetValue(source, name) as IEnumerable;
			IEnumerator enm = enumerable.GetEnumerator();
			while (index-- >= 0)
				enm.MoveNext();
			try
			{
				return enm.Current;
			}
			catch
			{
				/* Error fix
				InvalidOperationException: Operation is not valid due to the current state of the object
				error operation : add array element and GetValue() before construct.
				*/
				return null;
			}
		}
	}
}
#endif