
public class AskCube extends AskObject {
    double sideLength; 
    
    public AskCube(double side, double xCoord, double yCoord, int objectID, int userID, String deviceID){
        super(xCoord, yCoord, objectID, userID, deviceID);
        sideLength = side;
    }
}
