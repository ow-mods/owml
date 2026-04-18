using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OWML.Logging;
using UnityEngine;
using UnityEngine.Analytics;

namespace OWML.MenuExample
{
	public class DebugGUI : MonoBehaviour
	{
		public void OnGUI()
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}

			DrawGui();
		}

		private float lowest = 1.0f;

		private void DrawGui()
		{
			DrawCircle(new Vector2(200, 200), 200, Color.blue, 1);

			/*
			 * TWO BUTTONS
			 *
			 * Snaps to (0,0) and cardinal directions
			 * Magnitude not constrained
			 * Pressed threshold of 0.4
			 */

			var x = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindXButton);
			var xVal = OWInput.GetValue(x);
			var y = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindYButton);
			var yVal = OWInput.GetValue(y);

			DrawCircle(new Vector2(200 - xVal * 200, 200 - yVal * 200), 5, Color.red, 1);

			/*
			 * TWO AXIS
			 *
			 * Preference for cardinal directions
			 * Magnitude not constrained
			 */

			x = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindXAxis);
			xVal = OWInput.GetValue(x);
			y = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindYAxis);
			yVal = OWInput.GetValue(y);

			DrawCircle(new Vector2(200 - xVal * 200, 200 - yVal * 200), 6, Color.green, 1);

			// ----

			/*
			 * COMPOSITE OF TWO BUTTONS
			 */

			var comp = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindCompButton);
			var compVal = OWInput.GetAxisValue(comp);

			DrawCircle(new Vector2(200 + compVal.x * 200, 200 - compVal.y * 200), 7, Color.yellow, 1);

			/*
			 * COMPOSITE OF TWO AXIS
			 */

			comp = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindCompAxis);
			compVal = OWInput.GetAxisValue(comp);

			DrawCircle(new Vector2(200 + compVal.x * 200, 200 - compVal.y * 200), 8, Color.blue, 1);

			var button = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindSingleButton);
			var pressed = OWInput.IsPressed(button);
			var buttonVal = OWInput.GetValue(button);
			DrawRectangle(new Rect(10, 410, 200, 20), Color.white, true);
			DrawRectangle(new Rect(10, 410, 200 * buttonVal, 20), pressed ? Color.green : Color.white, false);
			GUI.Label(new Rect(220, 410, 100, 20), $"{buttonVal} - {lowest}");

			if (buttonVal < lowest && buttonVal > 0.001f)
			{
				lowest = buttonVal;
			}

			button = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindSingleButton07Threshold);
			pressed = OWInput.IsPressed(button);
			buttonVal = OWInput.GetValue(button);
			DrawRectangle(new Rect(10, 440, 200, 20), Color.white, true);
			DrawRectangle(new Rect(10, 440, 200 * buttonVal, 20), pressed ? Color.green : Color.white, false);
			GUI.Label(new Rect(220, 440, 100, 20), buttonVal.ToString());

			var axis = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindSingleAxis);
			pressed = OWInput.IsPressed(axis);
			var axisVal = OWInput.GetValue(axis);
			DrawRectangle(new Rect(10, 470, 200, 20), Color.white, true);
			DrawRectangle(new Rect(10, 470, 200 * axisVal, 20), pressed ? Color.green : Color.white, false);
			GUI.Label(new Rect(220, 470, 100, 20), axisVal.ToString());

			axis = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindSingleAxis07Threshold);
			pressed = OWInput.IsPressed(axis);
			axisVal = OWInput.GetValue(axis);
			DrawRectangle(new Rect(10, 500, 200, 20), Color.white, true);
			DrawRectangle(new Rect(10, 500, 200 * axisVal, 20), pressed ? Color.green : Color.white, false);
			GUI.Label(new Rect(220, 500, 100, 20), axisVal.ToString());
		}

		private Texture2D _tex;
		private void DrawLine(Vector2 p1, Vector2 p2, Color color, float width)
		{
			if (_tex == null)
			{
				_tex = new Texture2D(1, 1);
				_tex.SetPixel(0, 0, Color.white);
				_tex.Apply();
			}

			var oldMatrix = GUI.matrix;
			var oldColor = GUI.color;

			GUI.color = color;

			var angle = Vector3.Angle(p2 - p1, Vector2.right);
			if (p1.y > p2.y)
			{
				angle = -angle;
			}

			var length = (p2 - p1).magnitude;

			GUIUtility.RotateAroundPivot(angle, p1);
			GUI.DrawTexture(new Rect(p1.x, p1.y, length, width), _tex);

			GUI.matrix = oldMatrix;
			GUI.color = oldColor;
		}

		private void DrawCircle(Vector2 pos, float radius, Color color, float width)
		{
			const int precision = 16;
			const int angle = 360 / precision;

			var points = new Vector2[precision];
			for (var i = 0; i < precision; i++)
			{
				points[i] = new Vector2(pos.x + (radius * Mathf.Sin(angle * i * Mathf.Deg2Rad)), pos.y + (radius * Mathf.Cos(angle * i * Mathf.Deg2Rad)));
			}

			for (var i = 0; i < precision; i++)
			{
				if (i + 1 == precision)
				{
					DrawLine(points[i], points[0], color, width);
				}
				else
				{
					DrawLine(points[i], points[i + 1], color, width);
				}
			}
		}

		private void DrawRectangle(Rect rect, Color color, bool outline)
		{
			if (_tex == null)
			{
				_tex = new Texture2D(1, 1);
				_tex.SetPixel(0, 0, Color.white);
				_tex.Apply();
			}

			if (outline)
			{
				var a = new Vector2(rect.x, rect.y);
				var b = new Vector2(rect.xMax, rect.y);
				var c = new Vector2(rect.xMax, rect.yMax);
				var d = new Vector2(rect.x, rect.yMax);
				DrawLine(a, b, color, 1);
				DrawLine(b, c, color, 1);
				DrawLine(c, d, color, 1);
				DrawLine(d, a, color, 1);
			}
			else
			{
				var oldColor = GUI.color;
				GUI.color = color;
				GUI.DrawTexture(rect, _tex);
				GUI.color = oldColor;
			}
		}
	}
}
