using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using Vuforia;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using ASKLib;

class ASKWorker {

	public void FetchObjects (ConcurQueue<int> queue, int[] keys) {
		BinaryFormatter bf = new BinaryFormatter ();
		MemoryStream ms = new MemoryStream ();
		byte[] instream = new byte[100000];

		Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Connect (new IPEndPoint(IPAddress.Parse("10.9.169.218"), 1234));

		FetchQuery fq = new FetchQuery(new float[]{0, 0}, keys);
		bf.Serialize (ms, fq);
		socket.Send (ms.ToArray());

		instream = new byte[100000];
		socket.Receive (instream);
		ms = new MemoryStream (instream);
		object obj2 = bf.Deserialize (ms);

		if (obj2 is ObjectResult) {
			ObjectResult or = (ObjectResult) obj2;
			foreach(AskObject askobject in or.askObjects){
				queue.Enqueue (askobject.objectId);
			}
		} else {
			Debug.Log("Wrong kind of object.");
		}
	}

	public int InsertObject(byte[] obj, float[] coord){
		int answer = -5; 
		BinaryFormatter bf = new BinaryFormatter ();
		MemoryStream ms = new MemoryStream ();
		byte[] instream;

		Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Connect (new IPEndPoint(IPAddress.Parse("10.9.169.218"), 1234));

		InsertQuery iq = new InsertQuery(new AskObject(coord, 0, 0, obj, 0));

		ms = new MemoryStream();
		bf.Serialize(ms, iq);
		socket.Send(ms.ToArray());

		instream = new byte[100000];
		socket.Receive(instream);
		ms = new MemoryStream (instream);
		object ans = bf.Deserialize (ms);
		if (ans is BoolIntResult) {
			BoolIntResult ans2 = (BoolIntResult)ans;
			if (ans2.boolVal == true)
				answer = ans2.integer;
		} else {
			Debug.Log ("Wrong confirmation packet.");
		}
		socket.Close();
		return answer;
	}
}
