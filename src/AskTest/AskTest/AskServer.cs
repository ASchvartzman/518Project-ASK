using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using KdTree;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AskTest{
public class AskServer {

    // Incoming data from the client.
	Socket socket;
    //public static string data = null;
	double maxRadius = 1.0; 
	//public static Dictionary<int, Object> idMap;
	public static Dictionary<int, AskObject> idMap;
	public static int maxObjectId;
	public static int engaged=0;
	//  check where to define
	public static KdTree<double,int> KDTree;


	//Class constructor 
	public AskServer(Socket inputSocket){
	//Socket socket = inputSocket; 
	}
	// public AskServer(KdTree<int,int> kd,Dictionary<int, AskObject> map)
	// {
	// 	idMap=map;
	// 	KDTree=kd;
	// 	maxObjectId=0;
	// 	engaged.Exchange(false);
	// }
// What all do we need to check here
		Tuple<bool, int> InsertObject(InsertQuery insertQuery){
		// double x = insertQuery.askObject.getX(); 
		// double y = insertQuery.askObject.getY(); 
		AskObject obj=insertQuery.askObject;
		double[] coord=obj.position;
		//double radius = insertQuery.askObject.getRadius(); 
	
			//Boolean alwaysfalse = false;
			while (Interlocked.CompareExchange(ref engaged,1, 0)==1) {
			}
			try {
				KdTreeNode<double, int>[] neighbors = KDTree.RadialSearch(coord, 2*maxRadius, 100);
		
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
				//e.StackTrace();
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
			//e.StackTrace();
			return false;
		}
		return true;
	}

	AskObject[] FetchObject(FetchQuery fetchQuery){
		AskPredict predict=new AskPredict(fetchQuery);
		double[] centerPoint=predict.PredictTotal();
		double viewRadius=fetchQuery.viewRadius;
		int[] objectIds=fetchQuery.objectIds;
			List<AskObject> askobjects=new List<AskObject>();
		try {
				KdTreeNode<double, int>[] objects = KDTree.RadialSearch(centerPoint, viewRadius, 100);
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
			//e.StackTrace();
		}
		int count=askobjects.Count;
		AskObject[] result=new AskObject[count];
		for (int i=0;i<count;i++)
		{
			result[i]=askobjects[i];
		}
		
		return result;
	}


	/** Handle is the primary function that processes and categorizes all client requests.
     *
     * @param object Expects an instance of an object inheriting Query.
     * @return An object inheriting Result.
     */
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
	
//	/** This is the method that gets invoked on every thread's start().
//     * Defers the processing to Handle(), and handles the networking components.
//     */
	public void run(){
		try{
			byte[] instream = new byte[100000];
			socket.Receive(instream); 
				socket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(Handle(JsonConvert.DeserializeObject<Query>(Encoding.ASCII.GetString(instream))))));
			socket.Close();
		}
			catch (Exception e){
			//e.printStackTrace();
		}
	}
	
	public static void Main(String [] args){

		KDTree = new KdTree<double,int>(2);
		idMap = new Dictionary<int, AskObject>();
		maxObjectId = 0;
		//AskServer askServer = new AskServer(kdTree,idMap);

		try {
			TcpListener serverSocket = new TcpListener(1234);
			/** Waits to receive a client call. As soon as it gets one, forks a new instance to handle the same. */
			while(true){
				/** The accept() here is blocking. */
				Socket socket = serverSocket.AcceptSocket();
				AskServer askServer = new AskServer(socket);
				/** Start the thread. */
					Thread mythread=new Thread(askServer.run);
					mythread.Start();
				//askServer.Start();
			}
		}
		catch (Exception e){
			//e.StackTrace();
		}
	}
}

} 

public class Query {
	public int queryId;
}

/** TestQuery serves the purpose of debugging. */
class TestQuery:Query {
	/** String test -- Test message to be transmitted to the server. */
	public String test;

	public TestQuery(String inputTest){
		test = inputTest;
	}
}

/** InsertQuery allows the specification of an instance of AskObject to be inserted in to the database. */
public class InsertQuery: Query {
	/** AskObject askObject -- The object to be inserted. */
	public AskObject askObject;

	public InsertQuery(AskObject obj) {
		askObject = obj;
	}
}

/** InsertQuery contains objectId (a universal object identifier) to be deleted in to the database. */
class DeleteQuery: Query {
	/** int objectId -- The object to be deleted. */
	public int objectId;

	public DeleteQuery(int id) {
		objectId = id;
	}
}

/** In (near) future, FetchQuery will allow for specification of sensor data to pre-fetch objects. */
public class FetchQuery: Query {
	public double[] centerPoint = new double [2];
	public double[] speedVec = new double [3];
	// speed is v_x, v_y, v_{theta}
	//double viewAngle;
	public double viewRadius;
	//double compassAngle;

	public double RTT;
	public int[] objectIds;

	public FetchQuery(double[] _centerPoint, double[] _speedVec, double _viewRadius, double _RTT, int[] _objectIds) {
		centerPoint = _centerPoint;
		speedVec = _speedVec;
		//viewAngle = _viewAngle;
		viewRadius = _viewRadius;
		//compassAngle = _compassAngle;

		RTT = _RTT;
		objectIds = _objectIds;
	}



}

public class Result {

	public int queryId;
	//int userId;
	//String deviceId;
}

/** TestResult serves the purpose of debugging. */
class TestResult: Result {
	/** String test -- Test message to be transmitted to the client. */
	public String test;

	public TestResult(String inputString, int qId){
		test = inputString;
		queryId=qId;
	}
}

/** BoolResult allows the server to return a boolean field.
 * In the current implementation, this is the one and only valid return message for DeleteQuery. */
class BoolResult: Result {

	public bool boolVal;	
	public BoolResult(bool b, int qId) {
		boolVal = b;
		queryId=qId;
	}
}

/** BoolintResult allows the server to return a boolean field and an int field.
 * In the current implementation, this is the one and only valid return message for InsertQuery. */
class BoolIntResult: Result {

	public bool boolVal;
	public int integer;

	public BoolIntResult(bool b, int i, int qId) {
		boolVal = b;
		integer = i;
		queryId=qId;
	}
}

/** In (near) future, ObjectResult will allow the server to return a list of objects to the client. */
class ObjectResult: Result {

	public AskObject[] askObjects;

	public ObjectResult(AskObject[] _askObjects, int qId) {
		askObjects = _askObjects;
		queryId=qId;
	}
}

public class AskPredict {

	double[] centerPoint = new double[2];
	double[] speedVec = new double[3];
	//double viewAngle;
	double viewRadius;
	//double compassAngle;
	double RTT;

	public AskPredict(FetchQuery fetchQuery){
		centerPoint = fetchQuery.centerPoint;
		//viewAngle = fetchQuery.viewAngle;
		speedVec = fetchQuery.speedVec;
		//compassAngle = fetchQuery.compassAngle;
		viewRadius = fetchQuery.viewRadius;
		RTT = fetchQuery.RTT;
		//objectIds=fetchQuery.objectIds;
		//result.queryId=fetchQuery.queryId;
	}


	// Here compassAngle I'm assuming to be the angle of horizontal 
	// axis with rectangle covering vision
	public double[] PredictTotal(){
		double xCoord = centerPoint[0]+RTT*speedVec[0];
		double yCoord = centerPoint[1]+RTT*speedVec[1];
		// double cAngle=compassAngle+RTT*speedVec[2];
		// double x=viewRadius;
		// double y=2*viewRadius*Math.Tan(viewAngle/2);
		double[] queryPoints = new double [2];
		// works for angles less than 90 I think 
		// queryPoints[0] = xCoord - x*Math.Sin(cAngle)-y*Math.Cos(cAngle)/2;
		// queryPoints[1] = yCoord - y*Math.Sin(cAngle)/2; 
		// queryPoints[2] = xCoord +y*Math.Cos(cAngle)/2; 
		// queryPoints[3] = yCoord +x*Math.Cos(cAngle)+y*Math.Sin(cAngle)/2;
		queryPoints[0]=xCoord;
		queryPoints[1]=yCoord;
		return queryPoints; 

	}

}

public class AskObject 
{
	public string objectstream;
	public int userId;
	public int targetId;
	public double[] position=new double[2]; 
	public int objectId;

	public AskObject(double[] Coord, string obj, int userID, int objectID, int targetID){
		objectstream = obj;
		userId = userID;
		position=Coord;
		objectId=objectID;
		targetId=targetID;
	}

	/** Get the X coordinate of the center. */
	public double getX(){
		return position[0]; 
	}

	/** Get the Y coordinate for the center. */
	public double getY(){
		return position[1]; 
	}



	/** Returns the globalRadius. */
	// public double getRadius() {
	// 	return globalRadius;
	// }
}