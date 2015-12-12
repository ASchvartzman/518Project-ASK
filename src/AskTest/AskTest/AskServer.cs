using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using KdTree;
using KdTree.Math;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AskTest{
public class AskServer {

	Socket socket;
	float maxRadius = 1.0f; 
	public static Dictionary<int, AskObject> idMap;
	public static int maxObjectId;
	public static int engaged=0;
	public static KdTree<float,int> KDTree;

	public AskServer(Socket inputSocket){
		socket = inputSocket;
	}
	Tuple2<bool, int> InsertObject(InsertQuery insertQuery){
	AskObject obj=insertQuery.askObject;
	float[] coord=obj.position;
		
	while (Interlocked.CompareExchange(ref engaged,1, 0)==0) {
	}
		try {
		KdTreeNode<float, int>[] neighbors = KDTree.RadialSearch(coord, 2*maxRadius, 100);
		
		if (neighbors.Length>=1)
			{
			return new Tuple2<bool, int>(false, -1);
			}
				
			obj.objectId = maxObjectId+1;
			bool ifadd=KDTree.Add(obj.position, obj.objectId);
			if(!ifadd){
				return new Tuple2<bool, int>(false, -3);
			}
			idMap.Add(obj.objectId, obj);
			maxObjectId++;

		}
			catch (Exception e){
				Console.WriteLine (e.Message);
				return new Tuple2<bool, int>(false, -2);
			}
		/** Do not forget to hand over the lock. */
		engaged=0;
		KdTreeNode<float, int>[] myNeigh = KDTree.RadialSearch (new float[2] { 0.0f, 0.0f }, 1000000, 100); 
			Console.WriteLine ("Tree has " + myNeigh.Length.ToString () + " Children");  
		return new Tuple2<bool, int>(true, insertQuery.askObject.objectId);
	}
	
	bool DeleteObject(DeleteQuery deleteQuery){
		/** Checks if the received queryId is in idMap. */
		// TODO: 11/15/15  (Karan) It might be alright to say 'true'.
		if(!idMap.ContainsKey(deleteQuery.objectId))
			return false;
		
		AskObject askObject = idMap[deleteQuery.objectId];
		try{
			/** If the object is there, delete it. */
				int helpvalue;
				if (KDTree.TryFindValueAt(askObject.position,out helpvalue) == false)
				return false;
			KDTree.RemoveAt(askObject.position);
			idMap.Remove(deleteQuery.objectId);
		}
		catch (Exception e){
				Console.WriteLine (e.Message);
			return false;
		}
		return true;
	}

	AskObject[] FetchObject(FetchQuery fetchQuery){
		AskPredict predict=new AskPredict(fetchQuery);
		float[] centerPoint=predict.PredictTotal();
		float viewRadius=fetchQuery.viewRadius;
		int[] objectIds=fetchQuery.objectIds;
			foreach (KeyValuePair<int, AskObject> kvp in idMap) {
				Console.WriteLine ("Key = {0}, Val = {1} ", kvp.Key, kvp.Value); 
			} 	
			while (Console.KeyAvailable) {
				Console.ReadKey (true); 
			}
				
			int objID = int.Parse(Console.ReadLine());
			Console.WriteLine ("Fuck Karan " + objID.ToString()); 
				
		List<AskObject> askobjects=new List<AskObject>();
		askobjects.Add(idMap[objID]);
//		try {
//				int test = int.Parse(Console.ReadLine());
//				Console.WriteLine("The integer is " + test.ToString());
//				askobjects.Add(idMap[test]);
//				KdTreeNode<float, int>[] objects = KDTree.RadialSearch(centerPoint, viewRadius, 100);
//				for (int i=0;i<objects.Length;i++)
//			{
//				int objId=objects[i].Value;
//					bool present=false;
//					for (int j=0;j<objectIds.Length;j++)
//					{
//						if (objectIds[j]==objId)
//							present=true;
//					}
//				if(!present)
//				{
//					//Console.WriteLine(objId.ToString());
//					//askobjects.Add(idMap[objId]);
//				}
//			}
//		}
//		catch (Exception e) {
//			Console.WriteLine (e.StackTrace);
//		}
		
			return  askobjects.ToArray();
	}
			
	
	public void run(){
		try{
				byte[] instream = new byte[1000000];
				byte[] instream2 = new byte[1000000];
				socket.Receive(instream); 
				string encoding = Encoding.ASCII.GetString(instream);
				Console.WriteLine("APlace 1");
//				Console.WriteLine(encoding);
				Tuple2<string, string> queryObject = JsonConvert.DeserializeObject<Tuple2<string, string> >(encoding);
				Console.WriteLine("APlace 2");
				Tuple2<string, string> result;
				Console.WriteLine(queryObject.Item1);
				if (queryObject.Item1.Equals("TestQuery")) {
					TestQuery testQ = JsonConvert.DeserializeObject<TestQuery> (queryObject.Item2);
					Console.WriteLine ("Received a Test Query: " + ((TestQuery)testQ).test);
					TestResult testR = new TestResult("Indeed!", ((TestQuery)testQ).queryId);
					result = new Tuple2<string, string>("TestResult", JsonConvert.SerializeObject((TestResult)testR));
					socket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(result)));
				} else if (queryObject.Item1.Equals("InsertQuery")) {
					Console.WriteLine ("Received an Insert Query.");
					Console.WriteLine("APlace 3");
					InsertQuery insertQ = JsonConvert.DeserializeObject<InsertQuery>(queryObject.Item2);
					Console.WriteLine("APlace 4");
					socket.Receive(instream2);
					insertQ.askObject.objectstream = instream2;
					Tuple2<bool,int> result2 = InsertObject((InsertQuery) insertQ);
					BoolIntResult boolRes = new BoolIntResult (result2.Item1, result2.Item2, 0);
					Console.WriteLine("APlace 5");
					result = new Tuple2<string, string>("BoolIntResult", JsonConvert.SerializeObject((BoolIntResult) boolRes));
					Console.WriteLine("APlace Fuck Off");
					socket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(result)));

				} else if (queryObject.Item1.Equals("DeleteQuery")) {
					DeleteQuery deleteQ = JsonConvert.DeserializeObject<DeleteQuery> (queryObject.Item2);
					Console.WriteLine ("Received an Delete Query.");
					BoolResult result2 = new BoolResult(DeleteObject ((DeleteQuery)deleteQ), ((DeleteQuery)deleteQ).queryId);
					result = new Tuple2<string, string>("DeleteQuery", JsonConvert.SerializeObject((BoolResult) result2));
					socket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(result)));
				}

				else if(queryObject.Item1.Equals("FetchQuery")){
					FetchQuery fetchQ = JsonConvert.DeserializeObject<FetchQuery> (queryObject.Item2);
					Console.WriteLine("Received an Fetch Query.");
					Console.WriteLine("Place 0");
					AskObject[] thefuckingArray = FetchObject(fetchQ);
					Console.WriteLine("Place 1");
					byte[][] theotherFuckingArray = new byte[thefuckingArray.Length][];
					for(int i =0; i < thefuckingArray.Length; i++){
						theotherFuckingArray[i] = thefuckingArray[i].objectstream;
						thefuckingArray[i].objectstream = new byte[0];
					}
					ObjectResult result2 = new ObjectResult(thefuckingArray.Length, thefuckingArray,((FetchQuery)fetchQ).queryId);
					result = new Tuple2<string, string>("FetchQuery", JsonConvert.SerializeObject((ObjectResult) result2));
					Console.WriteLine("Place 2");
					socket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(result)));
					Console.WriteLine("Place 3");
					Console.WriteLine(thefuckingArray.Length.ToString());
						for (int i = 0; i < thefuckingArray.Length; i++){
							socket.Send(theotherFuckingArray[i]);
						} 
				}
				else {
					// TODO: 11/15/15   In case of a mismatch, I (Karan) recommend that this should throw an exception.
					Console.WriteLine("The Query Object wasn't of the right kind.");
					result = new Tuple2<string, string>("Don't know what to do", "0");
					socket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(result)));
				}

			}
			catch (Exception e){
				Console.WriteLine(e.StackTrace);
				Console.WriteLine(e.Message);
			}
//			socket.Close ();
	}
	
	public static void Main(String [] args){

		KDTree = new KdTree<float,int>(2, new FloatMath());

		idMap = new Dictionary<int, AskObject>();
		maxObjectId = 0;

		Socket listener = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		listener.Bind(new IPEndPoint(IPAddress.Any, 1234));
		listener.Listen(100);

		while (true) {
			Socket handler = listener.Accept ();
			AskServer askServer = new AskServer (handler);
			Thread mythread = new Thread (askServer.run);
			mythread.Start ();
		}
	}
}

} 



