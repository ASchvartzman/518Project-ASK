
public class AskObject extends Object {
    
    double x;
    double y;
    int objectId;
    int userId;
    String deviceId;

    
    public AskObject(double xCoord, double yCoord, int objectID, int userID, String deviceID){
        objectId = objectID;
        userId = userID; 
        deviceId = deviceID;
        x = xCoord; 
        y = yCoord; 
        
    }
    
    public double getX(){
        return x; 
    }
    
    public double getY(){
        return y; 
    }
    
     
}
