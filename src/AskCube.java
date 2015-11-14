
public class AskCube extends AskObject {
    double sideLength; 
    
    public AskCube(double side, double xCoord, double yCoord, int objectID, int userID, String deviceID, double globalRadius){
        super(xCoord, yCoord, objectID, userID, deviceID, globalRadius);
        sideLength = side;
    }
}
