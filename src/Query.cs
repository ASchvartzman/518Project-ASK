using System;
using System.Runtime.Serialization;
/** Query is an abstract class (with support for serialization).
 * All queries initiated from the client to the server must instantiate a class inheriting Query. */
[Serializable]
public class Query {
	int queryId;
}

/** TestQuery serves the purpose of debugging. */
class TestQuery:Query {
	/** String test -- Test message to be transmitted to the server. */
	String test;
	
	public TestQuery(String inputTest){
		test = inputTest;
	}
}

/** InsertQuery allows the specification of an instance of AskObject to be inserted in to the database. */
class InsertQuery: Query {
	/** AskObject askObject -- The object to be inserted. */
	AskObject askObject;
	
	public InsertQuery(AskObject obj) {
		askObject = obj;
	}
}

/** InsertQuery contains objectId (a universal object identifier) to be deleted in to the database. */
class DeleteQuery: Query {
	/** int objectId -- The object to be deleted. */
	int objectId;
	
	public DeleteQuery(int id) {
		objectId = id;
	}
}

/** In (near) future, FetchQuery will allow for specification of sensor data to pre-fetch objects. */
class public FetchQuery: Query {
	double[] centerPoint = new double [2];
	double[] speedVec = new double [3];
	// speed is v_x, v_y, v_{theta}
	//double viewAngle;
	double viewRadius;
	//double compassAngle;
	
	double RTT;
	int[] objectIds;
	
	// public FetchQuery(double[] _centerPoint, double[] _speedVec, double _viewAngle, double _viewRadius, double _compassAngle,  double _RTT, int[] _objectIds) {
	// 	centerPoint = _centerPoint;
	// 	speedVec = _speedVec;
	// 	viewAngle = _viewAngle;
	// 	viewRadius = _viewRadius;
	// 	compassAngle = _compassAngle;
		
	// 	RTT = _RTT;
	// 	objectIds = _objectIds;
	// }
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

