using System;
using System.Text;

namespace SharedMemory
{
	public delegate void ReceivedObjectHandler (String objString);
	public delegate void ReceivedCommandHandler (FeMoPeer source, String cmdString);

	public class FeMoPeer
	{
		private ConnectionHandle handle;

		public ConnectionHandle Handle {
			get {
				return handle;
			}
		}

		public event ReceivedCommandHandler ReceivedCommand;
		public event ReceivedObjectHandler ReceivedObject;

		protected virtual void OnReceivedCommand (String cmdString)
		{
			//Global.femo("Command: " + cmdString);
			if(ReceivedCommand != null)
				ReceivedCommand(this, cmdString);
		}

		protected virtual void OnReceivedObject (String objString)
		{
			if(ReceivedObject != null)
				ReceivedObject(objString);
		}

		public FeMoPeer (ConnectionHandle handle)
		{
			this.handle = handle;
			handle.ReceivedMessage += OnReceiveMessage;
		}

		private void OnReceiveMessage (String msgString)
		{
			msgString = msgString.Trim();
			String val = substring_workaround(msgString, msgString.IndexOf(":") + 2);
			if (msgString.StartsWith("obj:=")) {
				OnReceivedObject(val);
			} else if (msgString.StartsWith("cmd:=")) {
				OnReceivedCommand(val);
			} else {
				Global.warn("Unknown Message Type, make sure you are using compatible Versions\n\tType: \"" + msgString.Substring(0, 5) + "\"");
			}
		}


		public void Close ()
		{
			handle.Close();
		}

		public void SendObject(String objString) {
			handle.Send("obj:=" + objString);
		}

		public void SendCommand(String cmdString) {
			handle.Send("cmd:=" + cmdString);
		}

		private String substring_workaround (String str, int offset)
		{
			String ret = "";
			for (int i = 0; i < str.Length; i++) {
				if(i >= offset) {
					ret += str[i];
				}
			}
			return ret;
		}
	}
}

