using UnityEngine;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using Vuforia;

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

	public void Init () {

	}

	public void FetchObjects (ConcurQueue<string> queue) {
		Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Connect (new IPEndPoint(IPAddress.Parse("10.8.163.89"), 11000));
		byte[] instream = new byte[100000];
		int rec = socket.Receive (instream);
		queue.Enqueue (System.Text.Encoding.ASCII.GetString (instream));
	}
}

public class Client : MonoBehaviour {

	ASKWorker askWorker = new ASKWorker ();
	Thread clientThread;
	ConcurQueue<string> queue;

	// Use this for initialization
	void Start () {
		queue = new ConcurQueue<string> ();
		clientThread = new Thread (() => askWorker.FetchObjects(queue));
		clientThread.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		while (queue.Count > 0){
			string str = queue.Dequeue ();
			if(str.Substring(0,3).Equals("Red"))
			   GameObject.Find("ImageTarget/Cube").GetComponent<Renderer>().material.color = Color.red;
			else if(str.Substring(0,4).Equals("Blue"))
				GameObject.Find("ImageTarget/Cube").GetComponent<Renderer>().material.color = Color.blue;
			else
				GameObject.Find("ImageTarget/Cube").GetComponent<Renderer>().material.color = Color.white;
		}
		if (!clientThread.IsAlive) {
			clientThread = new Thread (() => askWorker.FetchObjects(queue));
			clientThread.Start ();
		}
	}
}
