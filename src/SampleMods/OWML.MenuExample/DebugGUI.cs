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

		private int _barY = 410;

		void DrawBar(InputConsts.InputCommandType type)
		{
			var command = InputLibrary.GetInputCommand(type);
			var pressed = OWInput.IsPressed(command);
			var val = OWInput.GetValue(command);

			DrawRectangle(new Rect(10, _barY, 200, 20), pressed ? Color.green : Color.white, true);
			DrawRectangle(new Rect(10, _barY, 200 * val, 20), pressed ? Color.green : Color.white, false);
			GUI.Label(new Rect(220, _barY, 100, 20), $"{val}");

			_barY += 30;
		}

		void DrawDualBar(InputConsts.InputCommandType type)
		{
			var command = InputLibrary.GetInputCommand(type);
			var pressed = OWInput.IsPressed(command);
			var val = OWInput.GetValue(command);

			DrawRectangle(new Rect(10, _barY, 400, 20), pressed ? Color.green : Color.white, true);
			DrawRectangle(new Rect(210, _barY, 200 * val, 20), pressed ? Color.green : Color.white, false);
			GUI.Label(new Rect(420, _barY, 100, 20), $"{val}");

			_barY += 30;
		}

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

			DrawCircle(new Vector2(200 + xVal * 200, 200 - yVal * 200), 5, Color.red, 1);

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

			DrawCircle(new Vector2(200 + xVal * 200, 200 - yVal * 200), 6, Color.green, 1);

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

			_barY = 410;

			DrawBar(GetComponent<MenuExample>().rebindSingleButton);
			DrawBar(GetComponent<MenuExample>().rebindSingleButton07Threshold);
			DrawBar(GetComponent<MenuExample>().rebindSingleAxis);
			DrawBar(GetComponent<MenuExample>().rebindSingleAxis07Threshold);

			DrawDualBar(GetComponent<MenuExample>().rebindDualButton);
			DrawDualBar(GetComponent<MenuExample>().rebindDualButton07Threshold);
			DrawDualBar(GetComponent<MenuExample>().rebindDualAxis);
			DrawDualBar(GetComponent<MenuExample>().rebindDualAxis07Threshold);

			DrawDualBar(InputLibrary.toolOptionX.CommandType);

			// Mouse inputs: buttons, delta (movement), and scroll
			DrawBar(GetComponent<MenuExample>().rebindMouseLeft);
			DrawBar(GetComponent<MenuExample>().rebindMouseRight);
			DrawBar(GetComponent<MenuExample>().rebindMouseMiddle);
			DrawDualBar(GetComponent<MenuExample>().rebindMouseExtra);

			var mx = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindDeltaX);
			var mxVal = OWInput.GetValue(mx);
			var my = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindDeltaY);
			var myVal = OWInput.GetValue(my);
			DrawCircle(new Vector2(200 + mxVal * 200, 200 - myVal * 200), 5, Color.magenta, 1);

			var sx = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindScrollX);
			var sxVal = OWInput.GetValue(sx);
			var sy = InputLibrary.GetInputCommand(GetComponent<MenuExample>().rebindScrollY);
			var syVal = OWInput.GetValue(sy);
			DrawCircle(new Vector2(200 + sxVal * 200, 200 - syVal * 200), 5, Color.cyan, 1);
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
