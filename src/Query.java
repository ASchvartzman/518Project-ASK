import java.io.*;

abstract class Query implements Serializable {

    int queryId;
    int userId;
    String deviceId;
}

class TestQuery extends Query {

    String test;

    public TestQuery(String inputTest){
        test = inputTest;
    }
}

class InsertQuery extends Query {

}

class DeleteQuery extends Query {

}

class FetchQuery extends Query {

}