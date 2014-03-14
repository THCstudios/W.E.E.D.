using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

namespace SharedMemory
{

	public delegate void ReceivedMessageHandler(String msg);

	public enum ConnectionType
	{
		WAIT, CONNECT
	}

	public class ConnectionHandle
	{
		const int portNo = 12345;
		private int port;
		char localAddr;
		char remoteAddr;

		public char LocalAddr {
			get {
				return localAddr;
			}
			set {
				localAddr = value;
			}
		}

		public char RemoteAddr {
			get {
				return remoteAddr;
			}
			set {
				remoteAddr = value;
			}
		}

		public event ReceivedMessageHandler ReceivedMessage;

		protected virtual void OnReceivedMessage (String msg)
		{
			//Global.femo("Received Message: " + msg);
			if (msg.StartsWith ("port")) {
				String tmp = msg.Split('t')[1];
				port = Int32.Parse(tmp);
			} else
				if(ReceivedMessage != null)
					ReceivedMessage(msg);
		}

		private Socket socket;
		private Thread receive;

		private static Socket s;

		private ConnectionHandle (Socket s)
		{
			this.socket = s;
			ForwardingPort = RemotePort;
			receive = new Thread(Receive);
			receive.Start();
		}

		public void Close() {
			ReceivedMessage = null;
			receive.Abort();
			Send(":::end");
			socket.Shutdown(SocketShutdown.Both);
			socket.Close ();
			socket = null;
		}

		void Receive ()
		{
			try {
				Regex regex = new Regex(";:;");
				byte[] buffer = new byte[1024 * 8];
				socket.ReceiveBufferSize = 1024 * 8;
				String msg = "";
				while(socket.Connected) {
					buffer = new byte[1024 * 8];
					socket.Receive(buffer);
					String tmp = "";
					for(int i = 0; i < 8 * 1024; i++) {
						if(buffer[i] != 0)
							tmp += (char)buffer[i];
					}
					if(tmp.Contains(":::end"))
						Close();
					if(tmp == "") {
					} else if(tmp.Contains(";:;")) {
						String eve = msg + tmp.Substring(0, tmp.LastIndexOf(";:;"));
						msg += tmp.Substring(tmp.LastIndexOf(";:;") + 3);
						String[] msgs = regex.Split(eve);
						foreach(String m in msgs) {
							//Global.femo(m);
							OnReceivedMessage(m);
						}
					} else {
						msg += tmp;
					}
				}
				Close();
			} catch (Exception e) {

			}
		}

		public void Send (String msg)
		{
			String t = msg + ";:;";
			byte[] tmp = Encoding.Unicode.GetBytes(t);
			Double blocks_ = (double)tmp.Length / (1024.0d * 8.0d);
			int blocks = (int)Math.Ceiling(blocks_);
			//Global.log("Sending package: \n\tBytes:" + tmp.Length + " \n\tPort: " + RemotePort + "\n\tIP: " + RemoteIP + "\n\tBlocks: " + blocks);
			socket.Send(tmp);
		}

		public String RemoteIP {
			get {
				return (socket.RemoteEndPoint as IPEndPoint).Address.ToString();
			}
		}

		public int RemotePort {
			get {
				return (socket.RemoteEndPoint as IPEndPoint).Port;
			}
		}

		public int ForwardingPort {
			get {
				return port;
			}
			set {
				port = value;
				Send("port" + value);
			}
		}

		public static void Reset ()
		{
			s = null;
		}

		public static ConnectionHandle Connect (ConnectionType type, String ip, int portNo)
		{
			try {
				switch (type) {
				case ConnectionType.WAIT:
					{
						if (s == null) {
							SocketPermission permission = new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts);
							s = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
							IPHostEntry ipHost = Dns.GetHostEntry ("");
							IPAddress ipAddr = ipHost.AddressList [0];
							IPEndPoint endPoint = new IPEndPoint (ipAddr, portNo);
							s.Bind (endPoint);
							s.Listen (24);
						}
						Socket socket = s.Accept ();
						return new ConnectionHandle (socket);
					}
				case ConnectionType.CONNECT:
					{
						SocketPermission permission = new SocketPermission(NetworkAccess.Connect, TransportType.Tcp, "", SocketPermission.AllPorts);
						Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						IPHostEntry ipHost = Dns.GetHostEntry (ip);
						socket.Connect (ipHost.AddressList [0], portNo);
						return new ConnectionHandle (socket);
					}
				default:
					return null;
				}
			} catch (Exception ex) {
				Console.WriteLine(ex);
				return null;
			}
		}

	}
}

