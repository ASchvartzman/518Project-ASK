
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

using ASKExpLib;
using System.Runtime.Serialization.Formatters.Binary;

namespace ASKExperiment{





//class ASKWorker {
//
//	public void FetchObjects (ConcurQueue<byte[]> queue) {
//		BinaryFormatter bf = new BinaryFormatter ();
//		MemoryStream ms = new MemoryStream ();
//		byte[] instream = new byte[100000];
//
//		Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//		socket.Connect (new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234));
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
		static Dictionary<int,AskObject> idMap=new Dictionary<int, AskObject>();// targetID-> AskObject
		static int fetchtime=400;
		static int deletetime=10000;
		static int querytime=2000;
		static int stop=0;
		public static void Start () {
			BinaryFormatter bf = new BinaryFormatter ();
			MemoryStream ms = new MemoryStream ();
			int numberofobjects = 30;
			//string[] str = new string[]{"ImageTarget", "ImageTarget (1)", "ImageTarget (2)"};
			for(int i = 0; i<numberofobjects; i++){
				Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect (new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234));
				int length = 10000;
				byte[] obj = new byte[length];
				Random rnd = new Random ();
				rnd.NextBytes (obj);

				InsertQuery iq = new InsertQuery(new AskObject(new float[]{i*5,i*5},1,i,obj,0));// Change here

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

		public static void ConstantFetch(){
			while (stop == 0) {
				BinaryFormatter bf = new BinaryFormatter ();
				MemoryStream ms = new MemoryStream ();
				byte[] instream = new byte[100000];
	
				Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect (new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 1234));
				Random rnd = new Random ();
				float x = Convert.ToSingle(150*rnd.NextDouble());
				FetchQuery fq = new FetchQuery (new float[]{ x, x });
				fq.objectIds=new int[0];
				bf.Serialize (ms, fq);
				socket.Send (ms.ToArray ());
	
				instream = new byte[100000];
				socket.Receive (instream);
				ms = new MemoryStream (instream);
				object obj2 = bf.Deserialize (ms);
	
				if (obj2 is ObjectResult) {
					ObjectResult or = (ObjectResult)obj2;
					//Console.WriteLine (or.askObjects.Length);
					foreach (AskObject askobject in or.askObjects) {
						if (idMap.ContainsKey(askobject.targetId)) {
							idMap[askobject.targetId] = askobject;
						} else {
							idMap.Add(askobject.targetId, askobject);
						}
					}
				} else {
					Console.WriteLine ("Wrong kind of object.");
				}
				Thread.Sleep(fetchtime);
			}
			
		}
		public static void DeleteSomeTargets(){
			while (stop == 0) {
				Thread.Sleep(deletetime);
				idMap.Clear();
				
				
			}
		}
public static void Main(String[] args) {
	//StartClient();
			Start();
			Thread mythread=new Thread(ConstantFetch);
			mythread.Start();
			Thread DeleteThread = new Thread (DeleteSomeTargets);
			DeleteThread.Start ();

	//return 0;
			double sum=0;

			for (int i = 0; i < 20; i++) {// Change here
				
				Thread.Sleep(querytime);
				if (idMap.ContainsKey(i))
				{
					sum=sum+0;
					Console.WriteLine("Doing Good");
				}
				else{
					Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.Connect (new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234));
					FetchQuery2 fq = new FetchQuery2(new float[]{i*5+0.1f, i*5});
					fq.queryId = i;
					BinaryFormatter bf = new BinaryFormatter ();
					MemoryStream ms = new MemoryStream ();
					bf.Serialize (ms, fq);
					Stopwatch sw = new Stopwatch();
					sw.Start();
					socket.Send (ms.ToArray());

					byte[] instream = new byte[100000];
					socket.Receive (instream);
					ms = new MemoryStream (instream);
					object obj2 = bf.Deserialize (ms);

					if (obj2 is ObjectResult2) {
						if (obj2 == null)
							Console.WriteLine ("Null returned");
						sw.Stop();
						TimeSpan time= sw.Elapsed;
						sum = sum + time.TotalSeconds;
					}
					socket.Close();
				}
				double average = sum / (i + 1);
				//Console.WriteLine ("Average fetching time={0}", average);
				
			}
			stop = 1;
			double aver = sum / 20;
			Console.WriteLine ("Average fetching time={0}", aver);
		}
	}
}
//Time left

// Query time=2000
// Server latency=0; 0.000861645
// Server latency=400; 0.28419646
// Server latency=800; 0.44268451
// Server latency=1200; =0.84418643
