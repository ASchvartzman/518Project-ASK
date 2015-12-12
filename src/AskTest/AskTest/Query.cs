using System;
using System.Runtime.Serialization;
/** Query is an abstract class (with support for serialization).
 * All queries initiated from the client to the server must instantiate a class inheriting Query. */

namespace AskTest{
[Serializable]
	class Tuple2<T,U> {

		public T Item1;
		public U Item2;
		public Tuple2(T i1, U i2){
			Item1 = i1;
			Item2 = i2;
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
		public float[] centerPoint = new float [2];
		public float[] speedVec = new float [3];
		// speed is v_x, v_y, v_{theta}
		//float viewAngle;
		public float viewRadius;
		//float compassAngle;

		public float RTT;
		public int[] objectIds;

		public FetchQuery(float[] _centerPoint, float[] _speedVec, float _viewRadius, float _RTT, int[] _objectIds) {
			centerPoint = _centerPoint;
			speedVec = _speedVec;
			//viewAngle = _viewAngle;
			viewRadius = _viewRadius;
			//compassAngle = _compassAngle;

			RTT = _RTT;
			objectIds = _objectIds;
		}



	}
}