
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
//using System.Runtime.Serialization.Formatters.Binary;

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
		static int deletetime=20000;
//		static int querytime=2000;
		static int stop=0;
		static int numberofobjects = 200;
		static bool move=true;
		static float[] objectX=new float[numberofobjects];
		static float[] objectY=new float[numberofobjects];
		static float[] currentLoc=new float[]{0f,0f};
		static float[] velocity=new float[]{1f,0f}; // 0.01 units/ms
		static int[] objectThere=new int[3];
		public static void updatelocation(){
			while (move) {
				Thread.Sleep (10);
				for (int i = 0; i < 2; i++) {
					currentLoc [i] = currentLoc [i] + velocity [i];
					if (currentLoc [i] >= 1000 || currentLoc [i] <= 0) {
						velocity [i] = -velocity [i];
					}
				}


			}

		}
		public static void updatevelocity(){
			while (move) {
				Thread.Sleep (200);
				Random rnd = new Random ();
				double theta = Math.PI * rnd.NextDouble ()-Math.PI/2;
				float previousv0 = velocity [0];
				velocity[0]=-(float)Math.Sin(theta)*velocity[1]+(float)Math.Cos(theta)*velocity[0];
				velocity[1]=(float)Math.Sin(theta)*previousv0+(float)Math.Cos(theta)*velocity[1];
				//Console.WriteLine ("({0},{1})",currentLoc[0],currentLoc[1]);

			}

		}
		public static int closeSee()
		{
			int minId=0;
			float minD = square (currentLoc [0] - objectX [0]) + square (currentLoc [1] - objectY [0]);
			for (int i = 1; i < numberofobjects; i++) {
				float dis=(square (currentLoc [0] - objectX [i]) + square (currentLoc [1] - objectY [i]));
				if (dis <= minD) {
					minId = i;
					minD = dis;
				}
			}
			return minId;
		}
		public static float square(float x)
		{
			return x * x;
		}
		public static void Start () {
			BinaryFormatter bf = new BinaryFormatter ();
			MemoryStream ms = new MemoryStream ();
			//int numberofobjects = 30;
			//string[] str = new string[]{"ImageTarget", "ImageTarget (1)", "ImageTarget (2)"};
			for(int i = 0; i<numberofobjects; i++){
				Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect (new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234));
				Random rndweight=new Random();
				int length = rndweight.Next(100,10000);
				byte[] obj = new byte[length];
				Random rnd = new Random ();
				rnd.NextBytes (obj);
				double xi = 1000 * rnd.NextDouble();
				double yi = 1000 * rnd.NextDouble();
				InsertQuery iq = new InsertQuery(new AskObject(new float[]{(float)xi,(float)yi},1,i,obj,0));

				ms = new MemoryStream();
				bf.Serialize(ms, iq);
				socket.Send(ms.ToArray());
				byte[] instream = new byte[100000];
				socket.Receive (instream);
				ms = new MemoryStream (instream);
				object obj2 = bf.Deserialize (ms);
				if (obj2 is BoolIntResult) {
					BoolIntResult or = (BoolIntResult) obj2;
					objectX[i]=(float) xi;
					objectY[i] = (float)yi;

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
				//Random rnd = new Random ();

				FetchQuery fq = new FetchQuery (currentLoc);
				fq.objectIds=objectThere;
				float[] speedVec=new float[3];
				speedVec [0] = velocity [0];
				speedVec [1] = velocity [1];
				speedVec [2] = 0;

				bf.Serialize (ms, fq);
				socket.Send (ms.ToArray ());
	
				instream = new byte[100000];
				socket.Receive (instream);
				ms = new MemoryStream (instream);
				object obj2 = bf.Deserialize (ms);
	
				if (obj2 is ObjectResult) {
					ObjectResult or = (ObjectResult)obj2;
					//Console.WriteLine (or.askObjects.Length);
					int j=0;
					foreach (AskObject askobject in or.askObjects) {
						if (idMap.ContainsKey(askobject.targetId)) {
							idMap[askobject.targetId] = askobject;

						} else {
							idMap.Add(askobject.targetId, askobject);
						}
						objectThere [j] = askobject.objectId;
						j = j + 1;

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
			int latency=00;

			//string myString = latency.ToString();

			Socket sock = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			sock.Connect (new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234));
			BinaryFormatter bf1 = new BinaryFormatter ();
			MemoryStream ms1 = new MemoryStream ();
			bf1.Serialize (ms1, latency);

			sock.Send (ms1.ToArray());
			sock.Close ();
			Start();
			Thread mythread=new Thread(ConstantFetch);
			mythread.Start();
			Thread DeleteThread = new Thread (DeleteSomeTargets);
			DeleteThread.Start ();
			Thread lmythread=new Thread(updatelocation);
			lmythread.Start();
			Thread vmythread=new Thread(updatevelocity);
			vmythread.Start();
	//return 0;
			double sum=0;

			for (int i = 0; i < 30; i++) {// Change here
				
				Random rn = new Random ();

				Thread.Sleep (rn.Next(300,1200));
				int targ = closeSee ();
				if (idMap.ContainsKey(targ))
				{
					sum=sum+0;
					Console.WriteLine("Doing Good");
				}
				else{
					Console.WriteLine ("Doing Bad");
					Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.Connect (new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234));
					FetchQuery2 fq = new FetchQuery2(new float[]{objectX[targ],objectY[targ]});
					fq.targetId = targ;
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
			double aver = sum / 30;
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
