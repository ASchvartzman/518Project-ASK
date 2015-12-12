using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using KdTree;
using KdTree.Math;

using ASKLib;

namespace ASKServer
{
	public class ASKServer
	{

		Socket socket;
		float maxRadius = 1.0f;
		public static Dictionary<int, AskObject> idMap;
		public static int maxObjectId;
		public static Object engaged = new Object();
		public static KdTree<float,int> KDTree;

		public ASKServer(Socket inputSocket)
		{
			socket = inputSocket;
		}

		Tuple<bool, int> InsertObject (InsertQuery insertQuery)
		{
			AskObject obj = insertQuery.askObject;
			float[] coord = obj.position;
		
			lock (engaged) {
				try {
					KdTreeNode<float, int>[] neighbors = KDTree.RadialSearch (coord, 2 * maxRadius, 100);
			
					if (neighbors.Length >= 1) {
						return new Tuple<bool, int> (false, -1);
					}
					
					obj.objectId = maxObjectId + 1;
					bool ifadd = KDTree.Add (obj.position, obj.objectId);
					if (!ifadd) {
						Console.WriteLine ("Object already present.");
						return new Tuple<bool, int> (false, -3);
					}
					idMap.Add (obj.objectId, obj);
					maxObjectId++;

				} catch (Exception e) {
					Console.WriteLine (e.Message);
					return new Tuple<bool, int> (false, -2);
				}
			}
			KdTreeNode<float, int>[] myNeigh = KDTree.RadialSearch (new float[2] { 0.0f, 0.0f }, 1000000, 100); 
			Console.WriteLine ("Tree has " + myNeigh.Length.ToString () + " Children");  
			return new Tuple<bool, int> (true, insertQuery.askObject.objectId);
		}

		AskObject[] FetchObject (FetchQuery fetchQuery) {
			return new AskObject[]{ idMap [3] };
		}

		bool DeleteObject (DeleteQuery deleteQuery)
		{
			// TODO: 11/15/15  (Karan) It might be alright to say 'true'.
			if (!idMap.ContainsKey (deleteQuery.objectId))
				return false;
		
			AskObject askObject = idMap [deleteQuery.objectId];
			try {
				int helpvalue;
				if (KDTree.TryFindValueAt (askObject.position, out helpvalue) == false)
					return false;
				KDTree.RemoveAt (askObject.position);
				idMap.Remove (deleteQuery.objectId);
			} catch (Exception e) {
				Console.WriteLine (e.Message);
				return false;
			}
			return true;
		}



		public object Handler(object obj){
			Object obj2;
			if (obj is TestQuery) {
				TestQuery testQ = (TestQuery) obj;
				Console.WriteLine ("Received a Test Query: " + testQ.test);

				obj2 = new TestResult ("Indeed!", testQ.queryId);
			} else if (obj is InsertQuery) {
				InsertQuery insertQ = (InsertQuery) obj;
				Console.WriteLine ("Received an Insert Query.");

				Tuple<bool,int> result = InsertObject (insertQ);
				obj2 = new BoolIntResult (result.Item1, result.Item2, insertQ.queryId);
			} else if (obj is DeleteQuery) {
				DeleteQuery deleteQ = (DeleteQuery) obj;
				Console.WriteLine ("Received an Delete Query.");

				obj2 = new BoolResult (DeleteObject (deleteQ), deleteQ.queryId);
			} else if (obj is FetchQuery) {
				FetchQuery fetchQ = (FetchQuery) obj;
				Console.WriteLine ("Received an Fetch Query.");

				AskObject[] askObjects = FetchObject (fetchQ);
				obj2 = new ObjectResult(askObjects, fetchQ.queryId);
			} else {
				// TODO: 11/15/15   In case of a mismatch, I (Karan) recommend that this should throw an exception.
				Console.WriteLine ("The Query Object wasn't of the right kind.");
				obj2 = new TestResult("Don't know what to do?", -1);
			}
			return obj2;
		}

	
		public void run ()
		{
			try {
				byte[] instream = new byte[1000000];
				socket.Receive (instream); 
				MemoryStream ms = new MemoryStream(instream);
				BinaryFormatter bf = new BinaryFormatter();
				object obj = bf.Deserialize(ms);

				object obj2 = Handler(obj);

				ms = new MemoryStream();
				bf.Serialize(ms, obj2);
				socket.Send(ms.ToArray());
			} catch (Exception e) {
				Console.WriteLine (e.StackTrace);
				Console.WriteLine (e.Message);
			}
			socket.Close ();
		}

		public static void Main (String[] args)
		{

			KDTree = new KdTree<float,int> (2, new FloatMath ());

			idMap = new Dictionary<int, AskObject> ();
			maxObjectId = 0;

			Socket listener = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listener.Bind (new IPEndPoint (IPAddress.Any, 1234));
			listener.Listen (100);

			while (true) {
				Socket handler = listener.Accept ();
				ASKServer askServer = new ASKServer (handler);
				Thread mythread = new Thread (askServer.run);
				mythread.Start ();
			}
		}
	}

}

//AskObject[] FetchObject (FetchQuery fetchQuery)
//{
//	//			AskPredict predict = new AskPredict (fetchQuery);
//	//			float[] centerPoint = predict.PredictTotal ();
//	//			float viewRadius = fetchQuery.viewRadius;
//	//			int[] objectIds = fetchQuery.objectIds;
//	foreach (KeyValuePair<int, AskObject> kvp in idMap) {
//		Console.WriteLine ("Key = {0}, Val = {1} ", kvp.Key, kvp.Value); 
//	} 	
//	while (Console.KeyAvailable) {
//		Console.ReadKey (true); 
//	}
//
//	int objID = 1;//int.Parse (Console.ReadLine ());
//
//	List<AskObject> askobjects = new List<AskObject> ();
//	askobjects.Add (idMap [objID]);
//	return askobjects.ToArray ();
//	//		try {
//	//				int test = int.Parse(Console.ReadLine());
//	//				Console.WriteLine("The integer is " + test.ToString());
//	//				askobjects.Add(idMap[test]);
//	//				KdTreeNode<float, int>[] objects = KDTree.RadialSearch(centerPoint, viewRadius, 100);
//	//				for (int i=0;i<objects.Length;i++)
//	//			{
//	//				int objId=objects[i].Value;
//	//					bool present=false;
//	//					for (int j=0;j<objectIds.Length;j++)
//	//					{
//	//						if (objectIds[j]==objId)
//	//							present=true;
//	//					}
//	//				if(!present)
//	//				{
//	//					//Console.WriteLine(objId.ToString());
//	//					//askobjects.Add(idMap[objId]);
//	//				}
//	//			}
//	//		}
//	//		catch (Exception e) {
//	//			Console.WriteLine (e.StackTrace);
//	//		}
//	//		
//	//			return  askobjects.ToArray ();
//}