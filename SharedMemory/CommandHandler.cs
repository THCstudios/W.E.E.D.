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
			//Global.AddCommandsJob(this);
		}

		void HandleReceivedCommand (FeMoPeer source, string cmdString)
		{
			String[] args = cmdString.Split(" ".ToCharArray());
			String cmd = args[0];
			//Global.log("Remote issued command: " + cmd + " origin: " + source.Handle.RemoteIP);
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

