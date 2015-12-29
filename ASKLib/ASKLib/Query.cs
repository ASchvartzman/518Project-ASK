using System;

namespace ASKLib {
	[Serializable]
	public class Query {

		public int queryId;
		int userId;
		String deviceId;
	}

	[Serializable]
	public class TestQuery:Query {

		public String test;

		public TestQuery(String inputTest){
			test = inputTest;
		}
	}

	[Serializable]
	public class InsertQuery: Query {

		public AskObject askObject;

		public InsertQuery(AskObject obj) {
			askObject = obj;
		}
	}

	[Serializable]
	public class DeleteQuery: Query {

		public int objectId;

		public DeleteQuery(int id) {
			objectId = id;
		}
	}

	[Serializable]
	public class FetchQuery: Query {

		public float[] centerPoint = new float [2];
		public float[] speedVec = new float [3];
		public float viewAngle;
		public float viewRadius;
		public float compassAngle;

		public float RTT;
		public int[] objectIds;

		public FetchQuery(float[] _centerPoint, int[] ids){
			centerPoint = _centerPoint;
			objectIds = ids;
		}
	}
}

