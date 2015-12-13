
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

using BasicLib;
using System.Runtime.Serialization.Formatters.Binary;


namespace Basic{





//class ASKWorker {
//
//	public void FetchObjects (ConcurQueue<byte[]> queue) {
//		BinaryFormatter bf = new BinaryFormatter ();
//		MemoryStream ms = new MemoryStream ();
//		byte[] instream = new byte[100000];
//
//		Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//		socket.Connect (new IPEndPoint(IPAddress.Parse("10.9.101.248"), 1234));
//
//		FetchQuery fq = new FetchQuery(new float[]{0, 0});
//		bf.Serialize (ms, fq);
//		socket.Send (ms.ToArray());
//
//		instream = new byte[100000];
//		socket.Receive (instream);
//		ms = new MemoryStream (instream);
//		object obj2 = bf.Deserialize (ms);
//
//		if (obj2 is ObjectResult) {
//			ObjectResult or = (ObjectResult) obj2;
//			Debug.Log(or.askObjects.Length);
//			foreach(AskObject askobject in or.askObjects){
//				queue.Enqueue(askobject.objectstream);
//			}
//		} else {
//			Debug.Log("Wrong kind of object.");
//		}
//	}
//
//	
//	}

//public class Client  {
//	LocationService location = new LocationService ();	
//	ASKWorker askWorker = new ASKWorker ();
//	Thread clientThread;
//	ConcurQueue<byte[]> queue;
//
//	void Start () {
//		queue = new ConcurQueue<byte[]> ();
//		clientThread = new Thread (() => askWorker.FetchObjects(queue));
//		clientThread.Start ();
//	}
//
//	void Update () {
//		while (queue.Count > 0){
//			byte[] newobj = queue.Dequeue();
//			newobj.LoadObjectTree();
//		}
//		if (!clientThread.IsAlive) {
//		}
//	}
//}
public class Client{
		public static void Start () {
			BinaryFormatter bf = new BinaryFormatter ();
			MemoryStream ms = new MemoryStream ();
			int numberofobjects = 30;
			//string[] str = new string[]{"ImageTarget", "ImageTarget (1)", "ImageTarget (2)"};
			for(int i = 0; i<numberofobjects; i++){
				Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect (new IPEndPoint(IPAddress.Parse("10.9.101.248"), 1234));
				int length = 10000;
				byte[] obj = new byte[length];
				Random rnd = new Random ();
				rnd.NextBytes (obj);

				InsertQuery iq = new InsertQuery(new AskObject(new float[]{i*10,i*10},1,i,obj,0));

				ms = new MemoryStream();
				bf.Serialize(ms, iq);
				socket.Send(ms.ToArray());
				byte[] instream = new byte[100000];
				socket.Receive (instream);
				ms = new MemoryStream (instream);
				object obj2 = bf.Deserialize (ms);

				if (obj2 is BoolIntResult) {
					BoolIntResult or = (BoolIntResult) obj2;
					Console.WriteLine(or.integer);

				} else {
					Console.WriteLine("Wrong kind of object.");
				}
				socket.Close();
			}
		}
public static void Main(String[] args) {
	//StartClient();
			Start();

	//return 0;
			double sum=0;
			Stopwatch sw = new Stopwatch();
			for (int i = 0; i < 30; i++) {
				Thread.Sleep (400);
				Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect (new IPEndPoint(IPAddress.Parse("10.9.101.248"), 1234));
				FetchQuery fq = new FetchQuery(new float[]{i*10+0.1f, i*10});
				fq.queryId = i;
				BinaryFormatter bf = new BinaryFormatter ();
				MemoryStream ms = new MemoryStream ();
				bf.Serialize (ms, fq);
				sw.Start();
				socket.Send (ms.ToArray());

				byte[] instream = new byte[100000];
				socket.Receive (instream);
				ms = new MemoryStream (instream);
				object obj2 = bf.Deserialize (ms);

				if (obj2 is ObjectResult) {
					if (obj2 == null)
						Console.WriteLine ("Null returned");
					sw.Stop();
					TimeSpan time= sw.Elapsed;
					sum = sum + time.TotalSeconds;
				}
				socket.Close();
			}

			double average = sum / 30;
			Console.WriteLine ("Average fetching time={0}", average);
		}
	}
}
//Time left