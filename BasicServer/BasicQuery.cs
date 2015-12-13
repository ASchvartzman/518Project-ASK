using System;

namespace BasicServer {
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

		public int targetId;
		public float[] centerPoint;

		public FetchQuery(float[] _centerPoint){
			centerPoint = _centerPoint;
		}
	}
}

