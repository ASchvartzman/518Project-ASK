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

}

class BoolIntResult extends Result {

}

class ObjectResult extends Result {

}