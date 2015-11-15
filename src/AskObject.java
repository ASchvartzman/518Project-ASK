import java.io.Serializable;

/** AskObject is an abstract class (with support for serialization).
 * All nodes in the KD tree contain an instance of a class inheriting AskObject. */
abstract class AskObject implements Serializable {
    /** globalRadius must provide an upper bound for the radius of a sphere centered at x,y containing the object. */
    int objectId, userId;
    double x, y, globalRadius;

    public AskObject(double xCoord, double yCoord, int objectID, int userID, double global){
        objectId = objectID;
        userId = userID;
        x = xCoord; 
        y = yCoord; 
        globalRadius = global;
        
    }

    /** Get the X coordinate of the center. */
    public double getX(){
        return x; 
    }

    /** Get the Y coordinate for the center. */
    public double getY(){
        return y; 
    }

    /** Returns the globalRadius. */
    public double getRadius() {
        return globalRadius;
    }
}

/** AskCube is class (extending AskObject) that specifies a cube. */
class AskCube extends AskObject {
    /** double sideLength -- Length of a side of a cube. */
    double sideLength;

    public AskCube(double side, double xCoord, double yCoord, int objectID, int userID, double globalRadius){
        super(xCoord, yCoord, objectID, userID, globalRadius);
        sideLength = side;
    }
}

/** AskSphere is class (extending AskObject) that specifies a sphere. */
class AskSphere extends AskObject{
    /** double radius -- Length of the radius of the sphere. */
    double radius;

    public AskSphere(double rad, double xCoord, double yCoord, int objectID, int userID, double globalRadius){
        super(xCoord, yCoord, objectID, userID, globalRadius);
        radius = rad;
    }

}
