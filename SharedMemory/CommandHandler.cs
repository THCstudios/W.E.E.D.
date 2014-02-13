using System;
using System.Collections.Generic;

namespace SharedMemory
{

	public delegate int RemoteExecutable(String[] args);

	public class CommandHandler
	{
		private Dictionary<String, RemoteExecutable> remotes = new Dictionary<string, RemoteExecutable>();

		public CommandHandler (FeMoManager fm)
		{
			foreach (FeMoPeer p in fm.GetConnections()) {
				p.ReceivedCommand += HandleReceivedCommand;
			}
			Global.AddCommandsJob(this);
		}

		void HandleReceivedCommand (FeMoPeer source, string cmdString)
		{
			Global.log("Remote issued command: " + cmdString + " origin: " + source.Handle.RemoteIP);
			String[] args = cmdString.Split(" ".ToCharArray());
			String cmd = args[0];
			if(remotes.ContainsKey(cmd))
				remotes[cmd](args);
		}

		public void PutMethod (String name, RemoteExecutable method)
		{
			if (!remotes.ContainsKey (name)) {
				remotes.Add (name, method);
			} else {
				remotes[name] += method;
			}
		}
	}
}

