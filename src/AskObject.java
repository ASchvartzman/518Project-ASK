
public class AskObject extends Object {
    
    double x;
    double y;
    int objectId;
    int userId;
    String deviceId;
    double globalRadius;

    
    public AskObject(double xCoord, double yCoord, int objectID, int userID, String deviceID, double global){
        objectId = objectID;
        userId = userID; 
        deviceId = deviceID;
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
    
    public double getRadius(){
        return globalRadius; 
    }
     
}
