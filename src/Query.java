import java.io.*;

/** Query is an abstract class (with support for serialization).
 * All queries initiated from the client to the server must instantiate a class inheriting Query. */
abstract class Query implements Serializable {
    int queryId;
}

/** TestQuery serves the purpose of debugging. */
class TestQuery extends Query {
    /** String test -- Test message to be transmitted to the server. */
    String test;

    public TestQuery(String inputTest){
        test = inputTest;
    }
}

/** InsertQuery allows the specification of an instance of AskObject to be inserted in to the database. */
class InsertQuery extends Query {
    /** AskObject askObject -- The object to be inserted. */
    AskObject askObject;

    public InsertQuery(AskObject obj) {
        askObject = obj;
    }
}

/** InsertQuery contains objectId (a universal object identifier) to be deleted in to the database. */
class DeleteQuery extends Query {
    /** int objectId -- The object to be deleted. */
    int objectId;

    public DeleteQuery(int id) {
        objectId = id;
    }
}

/** In (near) future, FetchQuery will allow for specification of sensor data to pre-fetch objects. */
class FetchQuery extends Query {
    double[] centerPoint = new double [2];
    double[] speedVec = new double [2];
    double viewAngle;
    double viewRadius;
    double compassAngle;
    double compassChange;
    double RTT;
    int[] objectIds;

    public FetchQuery(double[] _centerPoint, double[] _speedVec, double _viewAngle, double _viewRadius, double _compassAngle, double _compassChange, double _RTT, int[] _objectIds) {
        centerPoint = _centerPoint;
        speedVec = _speedVec;
        viewAngle = _viewAngle;
        viewRadius = _viewRadius;
        compassAngle = _compassAngle;
        compassChange = _compassChange;
        RTT = _RTT;
        objectIds = _objectIds;
    }
}