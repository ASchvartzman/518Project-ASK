import java.lang.Math;

public class AskPredict {
    /**
     * Class that performs predictions based on sensor hints (location, speed). 
     * Prediction is split into translational and rotational updates. 
     * This will return points for the server to query. 
     * 
     * Moreover, we also implement a function that can be used as an oracle to 
     * take out false positives introduced due to the aggressive querying. 
     */
    
    double[] centerPoint = new double [2]; 
    double[] speedVec = new double [2]; 
    double viewAngle;
    double viewRadius;
    double compassAngle;
    AskObject[] currentView = new AskObject[0];
    
     
    public AskPredict(double[] center,double[] speed, double view, double radius, double compass,AskObject[] current)
    {
        centerPoint = center; 
        viewAngle = view;
        speedVec = speed;
        compassAngle = compass;
        viewRadius = view; 
        currentView = current;
        
    }
    
    public double[] predictRotation(double compassChange){
       double xCoord = centerPoint[0];
       double yCoord = centerPoint[1];
       double[] queryPoints = new double [4];
       queryPoints[0] = xCoord - viewRadius*Math.sin(viewAngle/2)*Math.cos(compassChange); 
       queryPoints[1] = yCoord - viewRadius*Math.cos(viewAngle/2)*Math.sin(compassChange) - viewRadius*Math.sin(viewAngle/2)*Math.sin(compassChange); 
       queryPoints[2] = xCoord - viewRadius*Math.cos(viewAngle/2)*Math.sin(compassChange) - viewRadius*Math.sin(viewAngle/2)*Math.sin(compassChange); 
       queryPoints[3] = yCoord - viewRadius*Math.sin(viewAngle/2)*Math.cos(compassChange);        
       return queryPoints; 
    }
    
    public double[] predictTranslation(double[] velocity, double RTT){
        double xCoord = centerPoint[0];
        double yCoord = centerPoint[1];
        double xSpeed = velocity[0];
        double ySpeed = velocity[1]; 
        double[] queryPoints = new double [4];
        queryPoints[0] = xCoord + RTT*xSpeed-viewRadius*Math.sin(viewAngle/2); 
        queryPoints[1] = yCoord + RTT*ySpeed;
        queryPoints[2] = xCoord + RTT*xSpeed+viewRadius*Math.sin(viewAngle/2); 
        queryPoints[3] = yCoord + RTT*ySpeed+viewRadius*Math.cos(viewAngle/2);
        return queryPoints; 
    }
    
    public double[] separator(double[] candidatePoints){
    	double[] confirmedPoints = new double[]; 
    	for(i=0;i<len(candidatePoints.length();i++){
    		candidateX = candidatePoints[i][0]; 
    		candidateY = candidatePoints[i][1]; 
    		double slope = (candidateY-centerPoint[1])/(candidateX-centerPoint[0]); 
    		if(abs(slope) > Math.tan(90-viewAngle/2)){
    			confirmedPoints.add(candidatePoints[i]); 
    		}
    	}
    	return confirmedPoints; 
    }
}
