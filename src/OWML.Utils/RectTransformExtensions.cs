using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWML.Utils
{
	public static class RectTransformExtensions
	{
		public static void SetLeft(this RectTransform rt, float left)
		{
			rt.offsetMin = new Vector2(left, rt.offsetMin.y);
		}

		public static void SetRight(this RectTransform rt, float right)
		{
			rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
		}

		public static void SetTop(this RectTransform rt, float top)
		{
			rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
		}

		public static void SetBottom(this RectTransform rt, float bottom)
		{
			rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
		}

		public static void SetHeight(this RectTransform rt, float height)
		{
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}

		public static void SetPosY(this RectTransform rt, float posY)
		{
			rt.localPosition = new Vector3(rt.localPosition.x, posY, rt.localPosition.z);
		}
	}
}
