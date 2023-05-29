using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWML.Utils
{
	public static class ArrayExtensions
	{
		public static T[] Add<T>(this T[] array, T item)
			=> array.Concat(new T[] { item }).ToArray();

		public static T[] Remove<T>(this T[] array, T item)
		{
			var list = new List<T>(array);
			list.Remove(item);
			return list.ToArray();
		}
	}
}
