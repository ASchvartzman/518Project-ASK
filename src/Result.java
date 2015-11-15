import java.io.*;

abstract class Result implements Serializable {

    int queryId;
    int userId;
    String deviceId;
}

class TestResult extends Result {

    String test;

    public TestResult(String inputString){
        test = inputString;
    }
}

class BoolResult extends Result {

    boolean bool;

    public BoolResult(boolean b) {
        bool = b;
    }
}

class BoolIntResult extends Result {

    boolean bool;
    int integer;

    public BoolIntResult(boolean b, int i) {
        bool = b; integer = i;
    }
}

class ObjectResult extends Result {

}