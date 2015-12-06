using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Collections.ArrayList;

public class AskServer : Thread {

    // Incoming data from the client.
	Socket socket;
    //public static string data = null;
	double maxRadius = 1.0; 
	//public static Dictionary<int, Object> idMap;
	public static Dictionary<int, AskObject> idMap;
	public static int maxObjectId;
	public static Interlocked engaged;
	//  check where to define
	public static KdTree<int,int> KDTree;


	//Class constructor 
	public AskServer(Socket inputSocket){
		Socket socket = inputSocket; 
	}
	// public AskServer(KdTree<int,int> kd,Dictionary<int, AskObject> map)
	// {
	// 	idMap=map;
	// 	KDTree=kd;
	// 	maxObjectId=0;
	// 	engaged.Exchange(false);
	// }
// What all do we need to check here
	Dictionary<Boolean, Integer> InsertObject(InsertQuery insertQuery){
		// double x = insertQuery.askObject.getX(); 
		// double y = insertQuery.askObject.getY(); 
		AskObject obj=insertQuery.askObject;
		double coord=obj.position;
		//double radius = insertQuery.askObject.getRadius(); 
	

		while(!engaged.CompareExchange(false, true)){
			try {
				KdTreeNode<int, int>[] neighbors = KDtree.RadialSearch(coord, 2*maxRadius, 100);
				// for(int i = 0; i < neighbors.length; i++){
				// 	//AskObject neighbor = (AskObject) neighbors[i];
				// 	askObject neighbor=idMap[neighbors[i].Value];
				// 	/** If there exists a point which might overlap (determined from globalRadius), reject. */
				// 	double distance = Math.Sqrt( Math.Pow(x-neighbor.getX(), 2) + Math.Pow(y-neighbor.getY(), 2) );
				// 	// if(distance < neighbor.getRadius() + radius)
				// 	// 	return new Dictionary<bool, int>(false, -1);
				// }
				/** Otherwise, insert. */
				if (neighbors.length>1)
				{
					return new Dictionary<bool, int>(false, -1);
				}
				
				obj.objectId = maxObjectId+1;
				bool ifadd=KDtree.Add(obj.position, obj.objectId);
				if(!ifadd){
					return new Dictionary<bool, int>(false, -3);
				}
				idMap.Add(obj.objectId, obj);
				maxObjectId++;

			}
			catch (Exception e){
				e.StackTrace();
				return new Dictionary<bool, int>(false, -2);
			}
			/** Do not forget to hand over the lock. */
			engaged.Exchange(false);

			return new Dictionary<bool, int>(true, insertQuery.askObject.objectId);
		}
	}
	
	bool DeleteObject(DeleteQuery deleteQuery){
		/** Checks if the received queryId is in idMap. */
		// TODO: 11/15/15  (Karan) It might be alright to say 'true'.
		if(!idMap.ContainsKey(deleteQuery.objectId))
			return false;
		
		AskObject askObject = idMap[deleteQuery.objectId];
		try{
			/** If the object is there, delete it. */
			if (KDtree.TryFindValueAt(askObject.position,askObject.objectId) == false)
				return false;
			KDtree.RemoveAt(askObject.position);
			idMap.Remove(deleteQuery.objectId);
		}
		catch (Exception e){
			e.StackTrace();
			return false;
		}
		return true;
	}

	AskObject[] FetchObject(FetchQuery fetchQuery){
		AskPredict predict=new AskPredict(fetchQuery);
		double[] centerPoint=predict.PredictTotal();
		double viewRadius=fetchQuery.viewRadius;
		int[] objectIds=fetchQuery.objectIds;
		ArrayList askobjects=new ArrayList();
		try {
			KdTreeNode<int, int>[] objects = KDtree.RadialSearch(centerPoint, viewRadius, 100);
			for (int i=0;i<objects.length;i++)
			{
				int objId=objects[i].Value;
				if(objectIds.IndexOf(objId)==-1)
				{
					askobjects.Add(idMap(objId));
				}
			}
		}
		catch (Exception e) {
			e.StackTrace();
		}
		int count=askobjects.Count;
		AskObject[] result=new AskObject[count];
		for (int i=0;i<count;i++)
		{
			result[i]=(AskObject) askobjects[i];
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
			return new TestResult("Indeed!",TestQuery.queryId);
		}
		else if(queryObject is InsertQuery){
			Console.WriteLine("Received an Insert Query.");
			Dictionary<bool, int> result = InsertObject((InsertQuery) queryObject);
			return new BoolIntResult(result.getKey(), result.getValue(), InsertQuery.queryId);
		}
		else if(queryObject is DeleteQuery){
			Console.WriteLine("Received an Delete Query.");
			return new BoolResult(DeleteObject((DeleteQuery) queryObject), DeleteQuery.queryId);
		}
		else if(queryObject is FetchQuery){
			Console.WriteLine("Received an Fetch Query.");
			return new ObjectResult(FetchObject((FetchQuery)queryObject),FetchQuery.queryId);
		}
		else {
			// TODO: 11/15/15   In case of a mismatch, I (Karan) recommend that this should throw an exception.
			Console.WriteLine("The Query Object wasn't of the right kind.");
			return new TestResult("Don't know what to do?");
		}
	}
	
//	/** This is the method that gets invoked on every thread's start().
//     * Defers the processing to Handle(), and handles the networking components.
//     */
//	public void run(){
//		try{
//			ObjectOutputStream objectOutputStream = new ObjectOutputStream(socket.getOutputStream());
//			ObjectInputStream objectInputStream = new ObjectInputStream(socket.getInputStream());
//			Object inpObject = objectInputStream.readObject();
//			objectOutputStream.writeObject(Handle(inpObject));
//			objectInputStream.close();
//			objectOutputStream.close();
//			socket.close();
//		}
//		catch (Exception e){
//			e.printStackTrace();
//		}
//	}
	
	public static void main(String [] args){
		engaged = new Boolean(false);
		KDTree = new KdTree<int,int>(2);
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
				askServer.Start();
			}
		}
		catch (Exception e){
			e.StackTrace();
		}
	}
}

//    public static void StartListening() {
//        // Data buffer for incoming data.
//        byte[] bytes = new Byte[1024];
//
//        // Establish the local endpoint for the socket.
//        // Dns.GetHostName returns the name of the 
//        // host running the application.
//        IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
//        IPAddress ipAddress = ipHostInfo.AddressList[0];
//        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
//
//        // Create a TCP/IP socket.
//        Socket listener = new Socket(AddressFamily.InterNetwork,
//            SocketType.Stream, ProtocolType.Tcp );
//
//        // Bind the socket to the local endpoint and 
//        // listen for incoming connections.
//        try {
//            listener.Bind(localEndPoint);
//            listener.Listen(10);
//
//            // Start listening for connections.
//            while (true) {
//                Console.WriteLine("Waiting for a connection...");
//                // Program is suspended while waiting for an incoming connection.
//                Socket handler = listener.Accept();
//                data = null;
//
//                //An incoming connection needs to be processed.
//                // while (true) {
//                //     bytes = new byte[1024];
//                //     int bytesRec = handler.Receive(bytes);
//
//                //     data = Encoding.ASCII.GetString(bytes,0,bytesRec);
//                //      Console.WriteLine( "Text received : {0}", data);
//                //     if (data.IndexOf("<EOF>") > -1) {
//                //         break;
//                //     }
//                // }
//
//                // Show the data on the console.
//                Console.WriteLine( "Text received : {0}", data);
//
//                // Echo the data back to the client.
//                byte[] msg = Encoding.ASCII.GetBytes("Blue");
//
//                handler.Send(msg);
//                handler.Shutdown(SocketShutdown.Both);
//                handler.Close();
//            }
//
//        } catch (Exception e) {
//            Console.WriteLine(e.ToString());
//        }
//
//        Console.WriteLine("\nPress ENTER to continue...");
//        Console.Read();
//
//    }
//
//    public static int Main(String[] args) {
//        StartListening();
//        return 0;
//    }
//}