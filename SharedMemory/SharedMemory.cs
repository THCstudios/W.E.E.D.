using System;
using System.IO;
using System.Collections.Generic;

namespace SharedMemory
{

	public class ConnectionInformation
	{
		public int port;
		public String address;
		public int noOfClients;
	}

	public class SharedMemoryControl
	{

		private List<ConnectionHandle> list = new List<ConnectionHandle> ();
		private Dictionary<String, int> ports = new Dictionary<string, int>();
		private List<String> initializationData = new List<String>();
		private FeMoManager fmm;

		public FeMoManager Manager {
			get {
				return fmm;
			}
		}

		public enum ServerClientState
		{
			SERVER, CLIENT
		}
		private ServerClientState state;

		public ServerClientState State {
			set {
				state = value;
			}
			get {
				return state;
			}
		}

		private ConnectionInformation info;

		public ConnectionInformation Info {
			set {
				info = value;
			}
			get {
				return info;
			}
		}

		public void AddSource (String s)
		{
			initializationData.Add(s);
		}

		public void Start ()
		{
			if (state != null && info != null) {
				runServerClientPhase();
				Global.RunJobs();
				runSlaveMasterPhase();
			}
		}

		public void runServerClientPhase ()
		{
			Global.femo ("Entering Phase 0 - Server Client - Role " + state);
			if (state == ServerClientState.SERVER) {
				for (int i = 0; i < info.noOfClients; i++) {
					ConnectionHandle handle;
					handle = ConnectionHandle.Connect (ConnectionType.WAIT, info.address, info.port);
					System.Threading.Thread.Sleep(200);
					handle.Send("max_con:" + info.noOfClients + ";curr:" + (i + 1));
					foreach(ConnectionHandle h in list) {
						Global.femo ("Forwarding new Connection to " + h.RemoteIP + ":" + h.ForwardingPort);
						handle.Send("ip:" + h.RemoteIP + ";port:" + h.ForwardingPort);
					}
					list.Add(handle);
					if(i != 0) {
						bool block = true;
						handle.ReceivedMessage += delegate(string msg) {
							if(msg.Contains("ack")) {
								Global.log("Client joined!");
								block = false;
							}
						};
						Global.femo ("Waiting for Client to join!");
						while(block)
							System.Threading.Thread.Sleep(5);
					}
				}
				System.Threading.Thread.Sleep(250);
			} else if (state == ServerClientState.CLIENT) {
				int max_con = 0, curr = -1;
				ConnectionHandle handle;
				list.Add(handle = ConnectionHandle.Connect(ConnectionType.CONNECT, info.address, info.port));
				if(handle == null) {
					Global.fail("Connecting to Server failed.");
					Environment.Exit(1);
				}
				handle.ReceivedMessage += delegate(string msg) {
					String[] parts = msg.Split(";".ToCharArray());
					for(int i = 0; i < parts.Length; i++) {
						if(parts[i].StartsWith("max_con")) {
							max_con = Int32.Parse(parts[i].Split(":".ToCharArray())[1]);
						} else if (parts[i].StartsWith("curr")) {
							curr = Int32.Parse(parts[i].Split(":".ToCharArray())[1]);
						} else if (parts[i].StartsWith("ip")) {
							String address;
							int port;
							address = parts[i].Split(":".ToCharArray())[1];
							port = Int32.Parse(parts[i+1].Split(":".ToCharArray())[1]);
							Global.femo ("Connecting to " + address + ":" + port);
							list.Add(ConnectionHandle.Connect(ConnectionType.CONNECT, address, port));
							Global.femo ("Current Peers: " + list.Count + " Needed: " + (curr));
							if(list.Count == curr)
								list[0].Send("ack");
						}
					}
				};
				System.Threading.Thread.Sleep(250);
				if(curr == -1)
					throw new Exception("Server timed out");
				Global.femo ("Waiting for further connections");
				while(list.Count != curr) {
					System.Threading.Thread.Sleep(5);
					Global.log(list.Count + "");
				}
				for(int i = curr; i < max_con; i++) {
					error:
					ConnectionHandle h = ConnectionHandle.Connect(ConnectionType.WAIT, "0.0.0.0", info.port);
					if(h == null) {
						info.port++;
						Global.femo ("Switching to port: " + info.port);
						list[0].ForwardingPort = info.port;
						ConnectionHandle.Reset();
						goto error;
					} else {
						list.Add(h);
						Global.femo ("Client Connected");
					}
					
				}
			}
		}

		public void runSlaveMasterPhase ()
		{
			Global.femo ("Entering Phase 1 - Master Slave - Role " + state);
			fmm = new FeMoManager ();
			FeMoPeer fmp = null;
			foreach (var handle in list) {
				if(fmp == null)
					fmm.AddConnection (fmp = new FeMoPeer (handle));
				else
					fmm.AddConnection (new FeMoPeer (handle));
			}
			fmm.EnableCommandHandling();
			Global.ConnectionStartup.RunAll();
			if (state == ServerClientState.SERVER) {
				fmm.BroadcastCommand("range 0 9999 Z");
				fmm.AddRange(new MemorySpan(0, 9999, fmm, 'Z'));
				fmm.BroadcastCommand("range 260000 269999 Z");
				fmm.AddRange(new MemorySpan(260000, 269999, fmm, 'Z'));
				fmm.BroadcastCommand("range 270000 279999 Z");
				fmm.AddRange(new MemorySpan(270000, 279999, fmm, 'Z'));
				for (int i = 0; i < this.info.noOfClients; i++) {
					fmm.BroadcastCommand("range " + (10000 * (i + 1)) + " " + (10000 * (i + 1) + 9999) + " " + (char)(65 + i));
					fmm.AddRange(new MemorySpan(10000 * (i + 1), 10000 * (i + 1) + 9999, fmm, (char)(65 + i)));
				}
				foreach (String s in initializationData) {
					fmm.ReadFromString (s, Global.GetDefaultFormatter());
				}
				fmm.SendUpdateString();
				Global.femo ("Entering Phase 2 - Peer - Role " + state);
				fmm.BroadcastCommand("run");
			} else {
				bool block = true;
				fmp.ReceivedCommand += delegate(FeMoPeer peer, string cmdString) {
					if(cmdString == "run")
						block = false;
				};
				while(block)
					System.Threading.Thread.Sleep(5);
				Global.femo ("Entering Phase 2 - Peer - Role " + state);
			}
		}

		public void Close ()
		{
			fmm.Close();
		}
	}
}

