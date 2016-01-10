
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


public class Client{
		static int numberofobjects = 200;
		static bool move=true;
		static float[] objectX=new float[numberofobjects];
		static float[] objectY=new float[numberofobjects];
		static float[] currentLoc=new float[]{0f,0f};
		static float[] velocity=new float[]{1f,0f}; // 0.01 units/ms

		public static void Start () {
			BinaryFormatter bf = new BinaryFormatter ();
			MemoryStream ms = new MemoryStream ();

			// In a 1000 X 1000 room 
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
					//Console.WriteLine(or.integer);
					objectX[i]=(float) xi;
					objectY[i] = (float)yi;

				} else {
					Console.WriteLine("Wrong kind of object.");
				}
				socket.Close();
			}
		}
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
			Thread mythread=new Thread(updatelocation);
			mythread.Start();
			Thread vmythread=new Thread(updatevelocity);
			vmythread.Start();
	//return 0;
			double sum=0;

			for (int i = 0; i < 30; i++) {
				Random rn = new Random ();

				Thread.Sleep (rn.Next(300,1200));
				Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect (new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234));
				int targ = closeSee ();
				FetchQuery fq = new FetchQuery(new float[]{objectX[targ],objectY[targ]});
				fq.targetId = targ;
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

				if (obj2 is ObjectResult) {
					if (obj2 == null)
						Console.WriteLine ("Null returned");
					sw.Stop();
					TimeSpan time= sw.Elapsed;
					sum = sum + time.TotalMilliseconds;
				}
				socket.Close();
			}

			double average = sum / 30;
			Console.WriteLine ("Average fetching time={0}", average);
			move = false;
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
}
}
//Time left
//Latency=0; 0.002903065
//Latency=400; 0.404652015
//Latency=800;0.80683223
//Latency=1200; 1.20568885