
import java.lang.Math;
import java.util.AbstractMap;

/**
 * Class that performs predictions based on sensor hints (location, speed).
 * Prediction is split into translational and rotational updates.
 * This will return points for the server to query.
 *
 * Moreover, we also implement a function that can be used as an oracle to
 * take out false positives introduced due to the aggressive querying.
 */

public class AskPredict {
    
    double[] centerPoint = new double[2];
    double[] speedVec = new double[3];
    double viewAngle;
    double viewRadius;
    double compassAngle;
    double RTT;
     
    public AskPredict(FetchQuery fetchQuery){
        centerPoint = fetchQuery.centerPoint;
        viewAngle = fetchQuery.viewAngle;
        speedVec = fetchQuery.speedVec;
        compassAngle = fetchQuery.compassAngle;
        viewRadius = fetchQuery.viewRadius;
        RTT = fetchQuery.RTT;
    }
    
    public double[] PredictRotation(){
       double xCoord = centerPoint[0];
       double yCoord = centerPoint[1];
       double compassChange=RTT*speedVec[2];
       double[] queryPoints = new double [4];
       queryPoints[0] = xCoord - viewRadius*Math.sin(viewAngle/2)*Math.cos(compassChange); 
       queryPoints[1] = yCoord - viewRadius*Math.cos(viewAngle/2)*Math.sin(compassChange) - viewRadius*Math.sin(viewAngle/2)*Math.sin(compassChange); 
       queryPoints[2] = xCoord - viewRadius*Math.cos(viewAngle/2)*Math.sin(compassChange) - viewRadius*Math.sin(viewAngle/2)*Math.sin(compassChange); 
       queryPoints[3] = yCoord - viewRadius*Math.sin(viewAngle/2)*Math.cos(compassChange);        
       return queryPoints; 
    }
    
    public double[] PredictTranslation(){
        double xCoord = centerPoint[0];
        double yCoord = centerPoint[1];
        double xSpeed = speedVec[0];
        double ySpeed = speedVec[1];
        double[] queryPoints = new double [4];
        queryPoints[0] = xCoord + RTT*xSpeed-viewRadius*Math.sin(viewAngle/2); 
        queryPoints[1] = yCoord + RTT*ySpeed;
        queryPoints[2] = xCoord + RTT*xSpeed+viewRadius*Math.sin(viewAngle/2); 
        queryPoints[3] = yCoord + RTT*ySpeed+viewRadius*Math.cos(viewAngle/2);
        return queryPoints; 
    }
    // Here compassAngle I'm assuming to be the angle of horizontal 
    // axis with rectangle covering vision
    public double[] PredictTotal(){
    	double xCoord = centerPoint[0]+RTT*speedVec[0];
        double yCoord = centerPoint[1]+RTT*speedVec[1];
        double cAngle=compassAngle+RTT*speedVec[2];
        double x=viewRadius;
        double y=2*viewRadius*Math.tan(viewAngle/2)
        double[] queryPoints = new double [4];
        // works for angles less than 90 I think 
        queryPoints[0] = xCoord - x*Math.sin(cAngle)-y*Math.cos(cAngle)/2;
        queryPoints[1] = yCoord - y*Math.sin(cAngle)/2; 
        queryPoints[2] = xCoord +y*Math.cos(cAngle)/2; 
        queryPoints[3] = yCoord +x*Math.cos(cAngle)+y*Math.sin(cAngle)/2;
        return queryPoints; 
    	
    }
    public AbstractMap.SimpleEntry<double[], double[]> TotalPredict(){
        return new AbstractMap.SimpleEntry<>(new double[]{0, 0}, new double[]{0,0});
    }

    public boolean Separator(double[] candidatePoint){
        double candidateX = candidatePoint[0];
        double candidateY = candidatePoint[1];
        double slope = (candidateY-centerPoint[1])/(candidateX-centerPoint[0]);
        return (Math.abs(slope) > Math.tan(90-viewAngle/2));
    }
}
