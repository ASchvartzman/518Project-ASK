using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

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