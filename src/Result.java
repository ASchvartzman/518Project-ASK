import java.io.*;

/** Result is an abstract class (with support for serialization).
 * All results being returned from the server to the client must instantiate a class inheriting Result. */
abstract class Result implements Serializable {

    int queryId;
    int userId;
    String deviceId;
}

/** TestResult serves the purpose of debugging. */
class TestResult extends Result {
    /** String test -- Test message to be transmitted to the client. */
    String test;

    public TestResult(String inputString){
        test = inputString;
    }
}

/** BoolResult allows the server to return a boolean field.
 * In the current implementation, this is the one and only valid return message for DeleteQuery. */
class BoolResult extends Result {

    boolean bool;

    public BoolResult(boolean b) {
        bool = b;
    }
}

/** BoolintResult allows the server to return a boolean field and an int field.
 * In the current implementation, this is the one and only valid return message for InsertQuery. */
class BoolIntResult extends Result {

    boolean bool;
    int integer;

    public BoolIntResult(boolean b, int i) {
        bool = b; integer = i;
    }
}

/** In (near) future, ObjectResult will allow the server to return a list of objects to the client. */
class ObjectResult extends Result {

    AskObject[] askObjects;

    public ObjectResult(AskObject[] _askObjects) {
        askObjects = _askObjects;
    }
}