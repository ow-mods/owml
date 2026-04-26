using OWML.Common;
using OWML.Common.Enums;
using System;
using UnityEngine.InputSystem;
using static InputConsts;

namespace OWML.Utils
{
	public static class InputCommandExtensions
	{
		public static IInputCommands GetInputCommand(this InputCommandType type)
			=> InputLibrary.GetInputCommand(type);

		public static string GetInputCommandPath(this Key key)
		{
			return ControlPathConstants.Keyboard.DEVICE + ControlPathConstants.PATH_SEPARATOR + (key switch
			{
				Key.None => ControlPathConstants.NONE,

				Key.Space => ControlPathConstants.Keyboard.SPACE,
				Key.Enter => ControlPathConstants.Keyboard.ENTER,
				Key.Tab => ControlPathConstants.Keyboard.TAB,
				Key.Backquote => ControlPathConstants.Keyboard.BACKQUOTE,
				Key.Quote => ControlPathConstants.Keyboard.QUOTE,
				Key.Semicolon => ControlPathConstants.Keyboard.SEMICOLON,
				Key.Comma => ControlPathConstants.Keyboard.COMMA,
				Key.Period => ControlPathConstants.Keyboard.PERIOD,
				Key.Slash => ControlPathConstants.Keyboard.SLASH,
				Key.Backslash => ControlPathConstants.Keyboard.BACKSLASH,
				Key.LeftBracket => ControlPathConstants.Keyboard.LEFT_BRACKET,
				Key.RightBracket => ControlPathConstants.Keyboard.RIGHT_BRACKET,
				Key.Minus => ControlPathConstants.Keyboard.MINUS,
				Key.Equals => ControlPathConstants.Keyboard.EQUALS,

				>= Key.A and <= Key.Z => key.ToString().ToLowerInvariant(),

				>= Key.Digit1 and <= Key.Digit0 => ((int)key - (int)Key.Digit1 + 1) % 10,

				Key.LeftShift => ControlPathConstants.Keyboard.LEFT_SHIFT,
				Key.RightShift => ControlPathConstants.Keyboard.RIGHT_SHIFT,
				Key.LeftAlt => ControlPathConstants.Keyboard.LEFT_ALT,
				Key.RightAlt => ControlPathConstants.Keyboard.RIGHT_ALT,
				Key.LeftCtrl => ControlPathConstants.Keyboard.LEFT_CTRL,
				Key.RightCtrl => ControlPathConstants.Keyboard.RIGHT_CTRL,
				Key.LeftMeta => ControlPathConstants.Keyboard.LEFT_META,
				Key.RightMeta => ControlPathConstants.Keyboard.RIGHT_META,
				Key.ContextMenu => ControlPathConstants.Keyboard.CONTEXT_MENU,

				Key.Escape => ControlPathConstants.Keyboard.ESCAPE,
				Key.LeftArrow => ControlPathConstants.Keyboard.LEFT_ARROW,
				Key.RightArrow => ControlPathConstants.Keyboard.RIGHT_ARROW,
				Key.UpArrow => ControlPathConstants.Keyboard.UP_ARROW,
				Key.DownArrow => ControlPathConstants.Keyboard.DOWN_ARROW,
				Key.Backspace => ControlPathConstants.Keyboard.BACKSPACE,
				Key.PageDown => ControlPathConstants.Keyboard.PAGE_DOWN,
				Key.PageUp => ControlPathConstants.Keyboard.PAGE_UP,
				Key.Home => ControlPathConstants.Keyboard.HOME,
				Key.End => ControlPathConstants.Keyboard.END,
				Key.Insert => ControlPathConstants.Keyboard.INSERT,
				Key.Delete => ControlPathConstants.Keyboard.DELETE,

				Key.CapsLock => ControlPathConstants.Keyboard.CAPS_LOCK,
				Key.NumLock => ControlPathConstants.Keyboard.NUM_LOCK,
				Key.PrintScreen => ControlPathConstants.Keyboard.PRINT_SCREEN,
				Key.ScrollLock => ControlPathConstants.Keyboard.SCROLL_LOCK,
				Key.Pause => ControlPathConstants.Keyboard.PAUSE,

				Key.NumpadEnter => ControlPathConstants.Keyboard.NUMPAD_ENTER,
				Key.NumpadDivide => ControlPathConstants.Keyboard.NUMPAD_DIVIDE,
				Key.NumpadMultiply => ControlPathConstants.Keyboard.NUMPAD_MULTIPLY,
				Key.NumpadPlus => ControlPathConstants.Keyboard.NUMPAD_PLUS,
				Key.NumpadMinus => ControlPathConstants.Keyboard.NUMPAD_MINUS,
				Key.NumpadPeriod => ControlPathConstants.Keyboard.NUMPAD_PERIOD,
				Key.NumpadEquals => ControlPathConstants.Keyboard.NUMPAD_EQUALS,

				>= Key.Numpad0 and <= Key.Numpad9 => ControlPathConstants.Keyboard.NUMPAD + ((int)key - (int)Key.Numpad0),

				>= Key.F1 and <= Key.F12 => ControlPathConstants.Keyboard.F + ((int)key - (int)Key.F1 + 1),

				>= Key.OEM1 and <= Key.OEM5 => ControlPathConstants.Keyboard.OEM + ((int)key - (int)Key.OEM1 + 1),

				_ => throw new NotImplementedException(),
			});
		}

		public static string GetInputCommandPath(this MouseBinding binding)
		{
			return ControlPathConstants.Mouse.DEVICE + ControlPathConstants.PATH_SEPARATOR + (binding switch
			{
				MouseBinding.None => ControlPathConstants.NONE,

				MouseBinding.Left => ControlPathConstants.Mouse.LEFT_BUTTON,
				MouseBinding.Right => ControlPathConstants.Mouse.RIGHT_BUTTON,
				MouseBinding.Middle => ControlPathConstants.Mouse.MIDDLE_BUTTON,

				MouseBinding.Back => ControlPathConstants.Mouse.BACK_BUTTON,
				MouseBinding.Forward => ControlPathConstants.Mouse.FORWARD_BUTTON,

				MouseBinding.DeltaUp => ControlPathConstants.Mouse.DELTA_UP,
				MouseBinding.DeltaDown => ControlPathConstants.Mouse.DELTA_DOWN,
				MouseBinding.DeltaLeft => ControlPathConstants.Mouse.DELTA_LEFT,
				MouseBinding.DeltaRight => ControlPathConstants.Mouse.DELTA_RIGHT,

				MouseBinding.ScrollUp => ControlPathConstants.Mouse.SCROLL_UP,
				MouseBinding.ScrollDown => ControlPathConstants.Mouse.SCROLL_DOWN,
				MouseBinding.ScrollLeft => ControlPathConstants.Mouse.SCROLL_LEFT,
				MouseBinding.ScrollRight => ControlPathConstants.Mouse.SCROLL_RIGHT,

				MouseBinding.PositionUp => ControlPathConstants.Mouse.POSITION_UP,
				MouseBinding.PositionDown => ControlPathConstants.Mouse.POSITION_DOWN,
				MouseBinding.PositionLeft => ControlPathConstants.Mouse.POSITION_LEFT,
				MouseBinding.PositionRight => ControlPathConstants.Mouse.POSITION_RIGHT,

				_ => throw new NotImplementedException(),
			});
		}

		public static string GetInputCommandPath(this GamepadBinding binding)
		{
			return ControlPathConstants.Gamepad.DEVICE + ControlPathConstants.PATH_SEPARATOR + (binding switch
			{
				GamepadBinding.None => ControlPathConstants.NONE,

				GamepadBinding.DPadUp => ControlPathConstants.Gamepad.DPAD_UP,
				GamepadBinding.DPadDown => ControlPathConstants.Gamepad.DPAD_DOWN,
				GamepadBinding.DPadLeft => ControlPathConstants.Gamepad.DPAD_LEFT,
				GamepadBinding.DPadRight => ControlPathConstants.Gamepad.DPAD_RIGHT,

				GamepadBinding.UpButton => ControlPathConstants.Gamepad.BUTTON_NORTH,
				GamepadBinding.DownButton => ControlPathConstants.Gamepad.BUTTON_SOUTH,
				GamepadBinding.LeftButton => ControlPathConstants.Gamepad.BUTTON_EAST,
				GamepadBinding.RightButton => ControlPathConstants.Gamepad.BUTTON_WEST,

				GamepadBinding.LeftStickClick => ControlPathConstants.Gamepad.LEFT_STICK_PRESS,
				GamepadBinding.RightStickClick => ControlPathConstants.Gamepad.RIGHT_STICK_PRESS,

				GamepadBinding.LeftShoulder => ControlPathConstants.Gamepad.LEFT_SHOULDER,
				GamepadBinding.RightShoulder => ControlPathConstants.Gamepad.RIGHT_SHOULDER,

				GamepadBinding.Start => ControlPathConstants.Gamepad.START,
				GamepadBinding.Select => ControlPathConstants.Gamepad.SELECT,

				GamepadBinding.LeftTrigger => ControlPathConstants.Gamepad.LEFT_TRIGGER,
				GamepadBinding.RightTrigger => ControlPathConstants.Gamepad.RIGHT_TRIGGER,

				GamepadBinding.Share => ControlPathConstants.Gamepad.SHARE,
				GamepadBinding.SystemButton => ControlPathConstants.Gamepad.SYSTEM_BUTTON,

				GamepadBinding.LeftStickUp => ControlPathConstants.Gamepad.LEFT_STICK_UP,
				GamepadBinding.LeftStickDown => ControlPathConstants.Gamepad.LEFT_STICK_DOWN,
				GamepadBinding.LeftStickLeft => ControlPathConstants.Gamepad.LEFT_STICK_LEFT,
				GamepadBinding.LeftStickRight => ControlPathConstants.Gamepad.LEFT_STICK_RIGHT,

				GamepadBinding.RightStickUp => ControlPathConstants.Gamepad.RIGHT_STICK_UP,
				GamepadBinding.RightStickDown => ControlPathConstants.Gamepad.RIGHT_STICK_DOWN,
				GamepadBinding.RightStickLeft => ControlPathConstants.Gamepad.RIGHT_STICK_LEFT,
				GamepadBinding.RightStickRight => ControlPathConstants.Gamepad.RIGHT_STICK_RIGHT,

				_ => throw new NotImplementedException(),
			});
		}

		public static string GetInputCommandPath(this InputDevice device)
			=> device switch
			{
				Gamepad => ControlPathConstants.Gamepad.DEVICE,
				Mouse => ControlPathConstants.Mouse.DEVICE,
				Keyboard => ControlPathConstants.Keyboard.DEVICE,
				_ => $"<{device.path.TrimStart(ControlPathConstants.PATH_SEPARATOR_CHAR)}>",
			};

		public static string GetInputCommandPath(this InputControl control)
		{
			string devicePath = control.device.GetInputCommandPath();

			int startIndex = control.path.IndexOf(ControlPathConstants.PATH_SEPARATOR_CHAR, 1);

			return devicePath + control.path.Substring(startIndex);
		}
	}
}