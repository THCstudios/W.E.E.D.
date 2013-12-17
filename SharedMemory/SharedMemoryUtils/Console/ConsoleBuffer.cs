using System;

namespace SharedMemory
{
	public class ConsoleBuffer : Printable
	{

		private int bufferWidth;
		private int bufferHeight;
		private ConsoleDisplay display;
		private Character[,] buffer;
		private int counter;

		public ConsoleBuffer (ConsoleDisplay display)
		{
			if (display != null) {
				this.display = display;
				bufferWidth = display.GetWidth ();
				bufferHeight = display.GetHeight ();
				Console.WriteLine ("Connected with: " + display.GetName ());
				Character[,] tmp = new Character[bufferWidth, bufferHeight];
				buffer = tmp;
			} else {
				Console.Error.WriteLine("Not a valid Console Display!");
			}
		}

		class Character {
			char value;
			PrintProperties properties;
			public char Value {
				get {
					return value;
				}
				set {
					this.value = value;
				}
			}
			public PrintProperties Properties {
				get {
					return properties;
				}
				set {
					this.properties = value;
				}
			}
		}


		void Printable.Next() {
			if(counter != -1)
				counter++;
			if(counter >= (bufferWidth + 1) * (bufferHeight + 1))
				counter = -1;
		}

		void Printable.Reset() {
			counter = 0;
		}

		char Printable.Value() {
			int x, y;
			if(counter == -1)
				return (char)0;
			x = counter / bufferHeight;
			y = counter % bufferHeight;
			return buffer[x, y].Value;
		}

		PrintProperties Printable.Properties() {
			int x, y;
			if(counter == -1) {
				PrintProperties pp = new PrintProperties();
				pp.Color = Color.BLACK;
				pp.Attributes = Attributes.BOLD;
				return pp;
			}
			x = counter / bufferHeight;
			y = counter % bufferHeight;
			return buffer[x, y].Properties;
		}
	}

	public interface Printable {
		void Next();
		char Value();
		PrintProperties Properties();
		void Reset();
	}

	public interface ConsoleDisplay {
		int GetWidth();
		int GetHeight();
		String GetName();
		void MoveCursor(int posx, int posy);
	}

	public struct PrintProperties {
		Color color;
		public Color Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}
		Attributes attributes;
		public Attributes Attributes {
			get {
				return attributes;
			}
			set {
				this.attributes = value;
			}
		}
	}

	public enum Color {
		BLACK = 0,
		RED = 1,
		GREEN = 2,
		YELLOW = 3,
		BLUE = 4,
		MAGENTA = 5,
		CYAN = 6,
		WHITE = 7
	}

	public enum Attributes {
		BOLD,
		UNDERSCORE,
		BLINK,
		REVERSE,
		CONCEALED
	}


}

