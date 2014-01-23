using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
			if(ReceivedMessage != null)
				ReceivedMessage(msg);
		}

		private Socket socket;
		private Thread receive;

		private static Socket s;

		private ConnectionHandle (Socket s)
		{
			this.socket = s;
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
				byte[] buffer = new byte[1024];
				String msg = "";
				while(socket.Connected) {
					buffer = new byte[1024];
					socket.Receive(buffer);
					String tmp = Encoding.Unicode.GetString(buffer);
					if(tmp == ":::end")
						Close();
					if(tmp == "") {
					} else if(tmp.Contains(";:;")) {
						String eve = msg + tmp.Substring(0, tmp.IndexOf(";:;"));
						msg += tmp.Substring(tmp.IndexOf(";:;") + 3);
						OnReceivedMessage(eve);
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
			//Console.WriteLine(t);
			socket.Send(tmp);
		}

		public String RemoteIP {
			get {
				return socket.
			}
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

