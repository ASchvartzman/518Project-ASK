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

class ConcurQueue<T> {

	private Queue<T> queue = new Queue<T> ();

	public int Count {
		get {
			return queue.Count;
		}
	}

	public void Enqueue (T obj) {
		lock(queue) {
			queue.Enqueue(obj);
			Monitor.PulseAll(queue);
		}
	}

	public T Dequeue () {
		T t;
		lock (queue) {
			t = queue.Dequeue ();
			Monitor.PulseAll(queue);
		}
		return t;
	}
}

class ASKWorker {

	public void FetchObjects (ConcurQueue<byte[]> queue) {
		BinaryFormatter bf = new BinaryFormatter ();
		MemoryStream ms = new MemoryStream ();
		byte[] instream = new byte[100000];

		Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Connect (new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234));

		FetchQuery fq = new FetchQuery(new float[]{0, 0});
		bf.Serialize (ms, fq);
		socket.Send (ms.ToArray());

		instream = new byte[100000];
		socket.Receive (instream);
		ms = new MemoryStream (instream);
		object obj2 = bf.Deserialize (ms);

		if (obj2 is ObjectResult) {
			ObjectResult or = (ObjectResult) obj2;
			Debug.Log(or.askObjects.Length);
			foreach(AskObject askobject in or.askObjects){
				queue.Enqueue(askobject.objectstream);
			}
		} else {
			Debug.Log("Wrong kind of object.");
		}
	}

	public void InsertObject(){

	}
}

public class Client : MonoBehaviour {
	LocationService location = new LocationService ();	
	ASKWorker askWorker = new ASKWorker ();
	Thread clientThread;
	ConcurQueue<byte[]> queue;

	void Start () {
		queue = new ConcurQueue<byte[]> ();
		clientThread = new Thread (() => askWorker.FetchObjects(queue));
		clientThread.Start ();
	}
	
	void Update () {
		while (queue.Count > 0){
			byte[] newobj = queue.Dequeue();
			newobj.LoadObjectTree();
		}
		if (!clientThread.IsAlive) {
		}
	}
}
