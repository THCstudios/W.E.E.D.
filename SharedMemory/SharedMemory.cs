using System;
using System.Collections.Generic;

namespace SharedMemory
{
	public class SharedMemory
	{
		public enum ServerClientState
		{
			SERVER, CLIENT
		}

		public struct ConnectionInformation
		{
			public int port;
			public String address;
			public int noOfClients;
		}

		public static List<ConnectionHandle> ServerClientPhase (ServerClientState state, ConnectionInformation info)
		{
			List<ConnectionHandle> list = new List<ConnectionHandle> ();
			if (state == ServerClientState.SERVER) {
				for (int i = 0; i < info.noOfClients; i++) {
					ConnectionHandle handle;
					list.Add (handle = ConnectionHandle.Connect (ConnectionType.WAIT, info.address, info.port));
					foreach(ConnectionHandle h in list) {
						handle.Send("ip:" + h.
					}
				}
			} else if (state == ServerClientState.CLIENT) {

			}
			return list;
		}
	}
}

