using System;

namespace SharedMemory
{
	public delegate void ReceivedObjectHandler (String objString);
	public delegate void ReceivedCommandHandler (String cmdString);

	public class FeMoPeer
	{
		private ConnectionHandle handle;

		public event ReceivedCommandHandler ReceivedCommand;
		public event ReceivedObjectHandler ReceivedObject;

		protected virtual void OnReceivedCommand (String cmdString)
		{
			if(ReceivedCommand != null)
				ReceivedCommand(cmdString);
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
			switch (msgString.Substring (0, 5)) {
			case "obj:=": {
				OnReceivedObject(msgString.Substring(5));
				break;
			}
			case "cmd:=": {
				OnReceivedCommand(msgString.Substring(5));
				break;
			}
			default:
				Console.WriteLine("Unknown Message Type, make sure you are using compatible Versions\n\tType: \"" + msgString.Substring(0, 5) + "\"");
				break;
			}
		}

		public void SendObject(String objString) {
			handle.Send("obj:=" + objString);
		}

		public void SendCommand(String cmdString) {
			handle.Send("cmd:=" + cmdString);
		}
	}
}

