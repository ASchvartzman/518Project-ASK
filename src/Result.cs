// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Runtime.Serialization;


/** Result is an abstract class (with support for serialization).
 * All results being returned from the server to the client must instantiate a class inheriting Result. */
[Serializable]
abstract class Result {

	int queryId;
	int userId;
	String deviceId;
}

/** TestResult serves the purpose of debugging. */
class TestResult: Result {
	/** String test -- Test message to be transmitted to the client. */
	String test;
	
	public TestResult(String inputString){
		test = inputString;
	}
}

/** BoolResult allows the server to return a boolean field.
 * In the current implementation, this is the one and only valid return message for DeleteQuery. */
class BoolResult: Result {

	bool boolVal;	
	public BoolResult(bool b) {
		boolVal = b;
	}
}

/** BoolintResult allows the server to return a boolean field and an int field.
 * In the current implementation, this is the one and only valid return message for InsertQuery. */
class BoolIntResult: Result {

	bool boolVal;
	int integer;

	public BoolIntResult(bool b, int i) {
		boolVal = b;
		integer = i;
	}
}

/** In (near) future, ObjectResult will allow the server to return a list of objects to the client. */
class ObjectResult: Result {

	AskObject[] askObjects;

	public ObjectResult(AskObject[] _askObjects) {
		askObjects = _askObjects;
	}
}


