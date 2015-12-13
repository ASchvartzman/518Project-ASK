using System;

namespace BasicLib {
	[Serializable]
	public class Result {
		
		public int queryId;
		int userId;
		String deviceId;
	}
		
	[Serializable]
	public class TestResult: Result {

		public String test;

		public TestResult(String inputString, int qId){
			test = inputString;
			queryId=qId;
		}
	}
		
	[Serializable]
	public class BoolResult: Result {

		public bool boolVal;

		public BoolResult(bool b, int qId) {
			boolVal = b;
			queryId=qId;
		}
	}
		
	[Serializable]
	public class BoolIntResult: Result {

		public bool boolVal;
		public int integer;

		public BoolIntResult(bool b, int i, int qId) {
			boolVal = b;
			integer = i;
			queryId=qId;
		}
	}
		
	 [Serializable]
	 public class ObjectResult: Result {

	 	public AskObject askObject;

	 	public ObjectResult(AskObject _askObject, int qId) {
	 		askObject = _askObject;
	 		queryId=qId;
	 	}
	 }
}