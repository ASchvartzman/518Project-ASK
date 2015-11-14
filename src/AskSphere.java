
public class AskSphere extends AskObject{
    double radius; 
    
    public AskSphere(double rad, double xCoord, double yCoord, int objectID, int userID, String deviceID, double globalRadius){
        super(xCoord, yCoord, objectID, userID, deviceID, globalRadius);
        radius = rad;
    }

}
