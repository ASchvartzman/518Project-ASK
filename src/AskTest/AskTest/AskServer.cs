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
	}
	Tuple<bool, int> InsertObject(InsertQuery insertQuery){
	AskObject obj=insertQuery.askObject;
	float[] coord=obj.position;
		
	while (Interlocked.CompareExchange(ref engaged,1, 0)==1) {
	}
		try {
		KdTreeNode<float, int>[] neighbors = KDTree.RadialSearch(coord, 2*maxRadius, 100);
		
		if (neighbors.Length>1)
			{
			return new Tuple<bool, int>(false, -1);
			}
				
			obj.objectId = maxObjectId+1;
			bool ifadd=KDTree.Add(obj.position, obj.objectId);
			if(!ifadd){
				return new Tuple<bool, int>(false, -3);
			}
			idMap.Add(obj.objectId, obj);
			maxObjectId++;

		}
			catch (Exception e){
				return new Tuple<bool, int>(false, -2);
			}
		/** Do not forget to hand over the lock. */
		engaged=0;
		return new Tuple<bool, int>(true, insertQuery.askObject.objectId);
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
			return false;
		}
		return true;
	}

	AskObject[] FetchObject(FetchQuery fetchQuery){
		AskPredict predict=new AskPredict(fetchQuery);
		float[] centerPoint=predict.PredictTotal();
		float viewRadius=fetchQuery.viewRadius;
		int[] objectIds=fetchQuery.objectIds;
		List<AskObject> askobjects=new List<AskObject>();
		try {
				KdTreeNode<float, int>[] objects = KDTree.RadialSearch(centerPoint, viewRadius, 100);
				for (int i=0;i<objects.Length;i++)
			{
				int objId=objects[i].Value;
					bool present=false;
					for (int j=0;j<objectIds.Length;j++)
					{
						if (objectIds[j]==objId)
							present=true;
					}
				if(!present)
				{
					askobjects.Add(idMap[objId]);
				}
			}
		}
		catch (Exception e) {
			Console.WriteLine (e.StackTrace);
		}
		return  askobjects.ToArray();
	}
			
	Object Handle(Object queryObject){
		if(queryObject is TestQuery){
			Console.WriteLine("Received a Test Query: "+((TestQuery) queryObject).test);
				return new TestResult("Indeed!",((TestQuery) queryObject).queryId);
		}
		else if(queryObject is InsertQuery){
			Console.WriteLine("Received an Insert Query.");
			Tuple<bool, int> result = InsertObject((InsertQuery) queryObject);
				return new BoolIntResult(result.Item1, result.Item2, ((InsertQuery) queryObject).queryId);
		}
		else if(queryObject is DeleteQuery){
			Console.WriteLine("Received an Delete Query.");
				return new BoolResult(DeleteObject((DeleteQuery) queryObject),((DeleteQuery) queryObject).queryId);
		}
		else if(queryObject is FetchQuery){
			Console.WriteLine("Received an Fetch Query.");
				return new ObjectResult(FetchObject((FetchQuery)queryObject),((FetchQuery)queryObject).queryId);
		}
		else {
			// TODO: 11/15/15   In case of a mismatch, I (Karan) recommend that this should throw an exception.
			Console.WriteLine("The Query Object wasn't of the right kind.");
			return new TestResult("Don't know what to do?",0);
		}
	}
	
	public void run(){
		try{
			byte[] instream = new byte[100000];
			socket.Receive(instream); 
			socket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(Handle(JsonConvert.DeserializeObject<Query>(Encoding.ASCII.GetString(instream))))));
			socket.Close();
		}
		catch (Exception e){
			Console.WriteLine(e.StackTrace);
		}
	}
	
	public static void Main(String [] args){

		KDTree = new KdTree<float,int>(2, new FloatMath());
		KDTree.Add(new float[] {1.0f,1.0f}, 1);
		KDTree.Add(new float[] {2.0f,2.0f}, 2); 

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



