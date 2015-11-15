import java.io.*;

abstract class Query implements Serializable {
    int queryId;
}

class TestQuery extends Query {

    String test;

    public TestQuery(String inputTest){
        test = inputTest;
    }
}

class InsertQuery extends Query {

    AskObject askObject;

    public InsertQuery(AskObject obj) {
        askObject = obj;
    }
}

class DeleteQuery extends Query {

    int objectId;

    public DeleteQuery(int id) {
        objectId = id;
    }
}

class FetchQuery extends Query {

}