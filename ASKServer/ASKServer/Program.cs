using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ASKServer
{
	class SomeObject {
		public int number = 42;
	}

	class Program {

		public static int Main(String[] args) {
			Socket listener = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listener.Bind(new IPEndPoint(IPAddress.Any, 1234));
			listener.Listen(100);

			while (true) {
				Socket handler = listener.Accept();
				Console.Write ("Got a response!");
				SomeObject so = new SomeObject ();
				so.number = 21;
				handler.Send(Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(so)));
//				byte[] instream = new byte[100000];
//				handler.Receive (instream);
//				Console.Write (Encoding.ASCII.GetString (instream));
				handler.Shutdown(SocketShutdown.Both);
				handler.Close();
			}
		}
	}
}
