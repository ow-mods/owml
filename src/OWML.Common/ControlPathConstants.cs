namespace OWML.Common
{
	public static class ControlPathConstants
	{
		public const char PATH_SEPARATOR_CHAR = '/';
		public const string PATH_SEPARATOR = "/";

		// common axes / directions
		public const string X = "x";
		public const string Y = "y";

		public const string UP = "up";
		public const string DOWN = "down";
		public const string UP_CAP = "Up";
		public const string DOWN_CAP = "Down";
		public const string LEFT = "left";
		public const string RIGHT = "right";

		public const string MIDDLE = "middle";
		public const string BACK = "back";
		public const string FORWARD = "forward";

		public const string NORTH = "North";
		public const string SOUTH = "South";
		public const string EAST = "East";
		public const string WEST = "West";

		// misc
		public const string BUTTON = "button";
		public const string BUTTON_CAP = "Button";

		public const string NONE = "none";

		public static class Keyboard
		{
			public const string DEVICE = "<Keyboard>";

			// basic keys
			public const string SPACE = "space";
			public const string ENTER = "enter";
			public const string TAB = "tab";

			public const string BACKQUOTE = "backquote";
			public const string QUOTE = "quote";
			public const string SEMICOLON = "semicolon";
			public const string COMMA = "comma";
			public const string PERIOD = "period";
			public const string SLASH = "slash";
			public const string BACKSLASH = "backslash";

			public const string MINUS = "minus";
			public const string EQUALS = "equals";

			// modifiers
			public const string SHIFT = "Shift";
			public const string ALT = "Alt";
			public const string CTRL = "Ctrl";
			public const string META = "Meta";
			public const string BRACKET = "Bracket";

			public const string LEFT_SHIFT = LEFT + SHIFT;
			public const string RIGHT_SHIFT = RIGHT + SHIFT;
			public const string LEFT_ALT = LEFT + ALT;
			public const string RIGHT_ALT = RIGHT + ALT;
			public const string LEFT_CTRL = LEFT + CTRL;
			public const string RIGHT_CTRL = RIGHT + CTRL;
			public const string LEFT_META = LEFT + META;
			public const string RIGHT_META = RIGHT + META;
			public const string LEFT_BRACKET = LEFT + BRACKET;
			public const string RIGHT_BRACKET = RIGHT + BRACKET;

			// navigation
			public const string ARROW = "Arrow";
			public const string LEFT_ARROW = LEFT + ARROW;
			public const string RIGHT_ARROW = RIGHT + ARROW;
			public const string UP_ARROW = UP + ARROW;
			public const string DOWN_ARROW = DOWN + ARROW;

			public const string ESCAPE = "escape";
			public const string BACKSPACE = "backspace";

			public const string PAGE = "page";
			public const string PAGE_UP = PAGE + UP_CAP;
			public const string PAGE_DOWN = PAGE + DOWN_CAP;

			public const string HOME = "home";
			public const string END = "end";
			public const string INSERT = "insert";
			public const string DELETE = "delete";

			// locks
			public const string LOCK = "Lock";
			public const string CAPS_LOCK = "caps" + LOCK;
			public const string NUM_LOCK = "num" + LOCK;
			public const string SCROLL_LOCK = "scroll" + LOCK;

			// numpad
			public const string NUMPAD = "numpad";
			public const string NUMPAD_ENTER = NUMPAD + "Enter";
			public const string NUMPAD_DIVIDE = NUMPAD + "Divide";
			public const string NUMPAD_MULTIPLY = NUMPAD + "Multiply";
			public const string NUMPAD_PLUS = NUMPAD + "Plus";
			public const string NUMPAD_MINUS = NUMPAD + "Minus";
			public const string NUMPAD_PERIOD = NUMPAD + "Period";
			public const string NUMPAD_EQUALS = NUMPAD + "Equals";

			// misc
			public const string PRINT_SCREEN = "printScreen";
			public const string PAUSE = "pause";
			public const string CONTEXT_MENU = "contextMenu";
			public const string OEM = "OEM";
			public const string F = "f";
		}

		public static class Mouse
		{
			public const string DEVICE = "<Mouse>";

			// buttons
			public const string LEFT_BUTTON = LEFT + BUTTON_CAP;
			public const string RIGHT_BUTTON = RIGHT + BUTTON_CAP;
			public const string MIDDLE_BUTTON = MIDDLE + BUTTON_CAP;
			public const string BACK_BUTTON = BACK + BUTTON_CAP;
			public const string FORWARD_BUTTON = FORWARD + BUTTON_CAP;

			// delta
			public const string DELTA = "delta";
			public const string DELTA_X = DELTA + PATH_SEPARATOR + X;
			public const string DELTA_Y = DELTA + PATH_SEPARATOR + Y;

			public const string DELTA_UP = DELTA + PATH_SEPARATOR + UP;
			public const string DELTA_DOWN = DELTA + PATH_SEPARATOR + DOWN;
			public const string DELTA_LEFT = DELTA + PATH_SEPARATOR + LEFT;
			public const string DELTA_RIGHT = DELTA + PATH_SEPARATOR + RIGHT;

			// scroll
			public const string SCROLL = "scroll";
			public const string SCROLL_X = SCROLL + PATH_SEPARATOR + X;
			public const string SCROLL_Y = SCROLL + PATH_SEPARATOR + Y;

			public const string SCROLL_UP = SCROLL + PATH_SEPARATOR + UP;
			public const string SCROLL_DOWN = SCROLL + PATH_SEPARATOR + DOWN;
			public const string SCROLL_LEFT = SCROLL + PATH_SEPARATOR + LEFT;
			public const string SCROLL_RIGHT = SCROLL + PATH_SEPARATOR + RIGHT;

			// position
			public const string POSITION = "position";
			public const string POSITION_X = POSITION + PATH_SEPARATOR + X;
			public const string POSITION_Y = POSITION + PATH_SEPARATOR + Y;
		}

		public static class Gamepad
		{
			public const string DEVICE = "<Gamepad>";

			// dpad
			public const string DPAD = "dpad";
			public const string DPAD_X = DPAD + PATH_SEPARATOR + X;
			public const string DPAD_Y = DPAD + PATH_SEPARATOR + Y;
			public const string DPAD_UP = DPAD + PATH_SEPARATOR + UP;
			public const string DPAD_DOWN = DPAD + PATH_SEPARATOR + DOWN;
			public const string DPAD_LEFT = DPAD + PATH_SEPARATOR + LEFT;
			public const string DPAD_RIGHT = DPAD + PATH_SEPARATOR + RIGHT;

			// face buttons
			public const string BUTTON_NORTH = BUTTON + NORTH;
			public const string BUTTON_SOUTH = BUTTON + SOUTH;
			public const string BUTTON_EAST = BUTTON + EAST;
			public const string BUTTON_WEST = BUTTON + WEST;

			// sticks
			public const string STICK = "Stick";
			public const string PRESS = "Press";
			public const string LEFT_STICK = LEFT + STICK;
			public const string RIGHT_STICK = RIGHT + STICK;

			// left stick
			public const string LEFT_STICK_X = LEFT_STICK + PATH_SEPARATOR + X;
			public const string LEFT_STICK_Y = LEFT_STICK + PATH_SEPARATOR + Y;
			public const string LEFT_STICK_UP = LEFT_STICK + PATH_SEPARATOR + UP;
			public const string LEFT_STICK_DOWN = LEFT_STICK + PATH_SEPARATOR + DOWN;
			public const string LEFT_STICK_LEFT = LEFT_STICK + PATH_SEPARATOR + LEFT;
			public const string LEFT_STICK_RIGHT = LEFT_STICK + PATH_SEPARATOR + RIGHT;
			public const string LEFT_STICK_PRESS = LEFT_STICK + PRESS;

			// right stick
			public const string RIGHT_STICK_X = RIGHT_STICK + PATH_SEPARATOR + X;
			public const string RIGHT_STICK_Y = RIGHT_STICK + PATH_SEPARATOR + Y;
			public const string RIGHT_STICK_UP = RIGHT_STICK + PATH_SEPARATOR + UP;
			public const string RIGHT_STICK_DOWN = RIGHT_STICK + PATH_SEPARATOR + DOWN;
			public const string RIGHT_STICK_LEFT = RIGHT_STICK + PATH_SEPARATOR + LEFT;
			public const string RIGHT_STICK_RIGHT = RIGHT_STICK + PATH_SEPARATOR + RIGHT;
			public const string RIGHT_STICK_PRESS = RIGHT_STICK + PRESS;

			// shoulders
			public const string SHOULDER = "Shoulder";
			public const string LEFT_SHOULDER = LEFT + SHOULDER;
			public const string RIGHT_SHOULDER = RIGHT + SHOULDER;

			// triggers
			public const string TRIGGER = "Trigger";
			public const string LEFT_TRIGGER = LEFT + TRIGGER;
			public const string RIGHT_TRIGGER = RIGHT + TRIGGER;

			// misc
			public const string START = "start";
			public const string SELECT = "select";
			public const string SHARE = "share";
			public const string SYSTEM_BUTTON = "system" + BUTTON_CAP;
		}
	}
}
