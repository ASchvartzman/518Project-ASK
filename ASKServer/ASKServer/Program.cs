using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using AskTest;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ASKServer
{
	[System.Serializable]
	class SomeObject {
		public int number = 42;
	}

	class Program {

		public static int Main(String[] args) {
			SomeObject so = new SomeObject();
			so.number = 12;
			byte[] instream = new byte[1000000]; 
			MemoryStream ms = new MemoryStream();
			BinaryFormatter bf = new BinaryFormatter ();
			bf.Serialize (ms, so);
			instream = ms.ToArray ();

			ms = new MemoryStream (instream);
			object obj = bf.Deserialize (ms);
			if (obj is SomeObject)
				Console.WriteLine (((SomeObject)obj).number);
//			Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//			socket.Connect (new IPEndPoint(IPAddress.Parse("10.9.101.248"), 1234));
//			socket.Send (Encoding.ASCII.GetBytes (JsonConvert.SerializeObject (new Tuple<string, string>("InsertQuery", JsonConvert.SerializeObject(new  InsertQuery(new AskObject(new float[]{0,0}, "", 0, 0, 0)))))));
//			byte[] instream = new byte[100000];
//			socket.Receive (instream);
//			Tuple<string, string> so = Newtonsoft.Json.JsonConvert.DeserializeObject<Tuple<string, string> > (Encoding.ASCII.GetString(instream));
//			Console.WriteLine (so.Item1);
//			Console.WriteLine (JsonConvert.DeserializeObject<BoolIntResult> (so.Item2).integer);
//
//			socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//			socket.Connect (new IPEndPoint(IPAddress.Parse("10.9.101.248"), 1234));
//			socket.Send (Encoding.ASCII.GetBytes (JsonConvert.SerializeObject (new Tuple<string, string>("DeleteQuery", JsonConvert.SerializeObject(new  DeleteQuery(1))))));
//			instream = new byte[100000];
//			socket.Receive (instream);
//			so = Newtonsoft.Json.JsonConvert.DeserializeObject<Tuple<string, string> > (Encoding.ASCII.GetString(instream));
//			Console.WriteLine (so.Item1);
//			Console.WriteLine (JsonConvert.DeserializeObject<BoolResult> (so.Item2).boolVal);

			//Console.WriteLine (so.integer);
//			socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//			socket.Connect (new IPEndPoint(IPAddress.Parse("10.9.101.248"), 1234));
//			socket.Send (Encoding.ASCII.GetBytes (JsonConvert.SerializeObject (new Tuple<string, string>("InsertQuery", JsonConvert.SerializeObject(new  InsertQuery(new AskObject(new float[]{2,2}, "", 0, 0, 0)))))));
//			instream = new byte[100000];
//			socket.Receive (instream);
//			so = Newtonsoft.Json.JsonConvert.DeserializeObject<BoolIntResult> (Encoding.ASCII.GetString(instream));
//			Console.WriteLine (so.integer);
//			socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//			socket.Connect (new IPEndPoint(IPAddress.Parse("10.9.101.248"), 1234));
//			socket.Send (Encoding.ASCII.GetBytes (JsonConvert.SerializeObject (new Tuple<string, string>("FetchQuery", JsonConvert.SerializeObject(new FetchQuery(new float[]{0,0}, new float[]{0,0}, 100, 100, new int[0]))))));
//			instream = new byte[100000];
//			socket.Receive (instream);
//			ObjectResult so2 = Newtonsoft.Json.JsonConvert.DeserializeObject<ObjectResult> (Encoding.ASCII.GetString(instream));
//			Console.WriteLine (so2.askObjects.Length);
			return 0;
		}
	}
}
