using System;

namespace AskTest{
/** Result is an abstract class (with support for serialization).
 * All results being returned from the server to the client must instantiate a class inheriting Result. */

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
		public int length; 

		public ObjectResult(int l, AskObject[] _askObjects, int qId) {
			length = l;
			askObjects = _askObjects;
			queryId=qId;
		}
	}
}