import java.io.Serializable;

abstract class AskObject implements Serializable {

    int objectId, userId;
    double x, y, globalRadius;

    public AskObject(double xCoord, double yCoord, int objectID, int userID, double global){
        objectId = objectID;
        userId = userID;
        x = xCoord; 
        y = yCoord; 
        globalRadius = global;
        
    }
    
    public double getX(){
        return x; 
    }
    
    public double getY(){
        return y; 
    }
    
    public double getRadius() { return globalRadius; }
}

class AskCube extends AskObject {

    double sideLength;

    public AskCube(double side, double xCoord, double yCoord, int objectID, int userID, double globalRadius){
        super(xCoord, yCoord, objectID, userID, globalRadius);
        sideLength = side;
    }
}

class AskSphere extends AskObject{
    double radius;

    public AskSphere(double rad, double xCoord, double yCoord, int objectID, int userID, double globalRadius){
        super(xCoord, yCoord, objectID, userID, globalRadius);
        radius = rad;
    }

}
