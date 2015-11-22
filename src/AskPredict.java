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
    double[] speedVec = new double[2];
    double viewAngle;
    double viewRadius;
    double compassAngle;
    double compassChange;
    double RTT;
     
    public AskPredict(FetchQuery fetchQuery){
        centerPoint = fetchQuery.centerPoint;
        viewAngle = fetchQuery.viewAngle;
        speedVec = fetchQuery.speedVec;
        compassAngle = fetchQuery.compassAngle;
        viewRadius = fetchQuery.viewRadius;
        compassChange = fetchQuery.compassChange;
        RTT = fetchQuery.RTT;
    }
    
    public double[] PredictRotation(){
       double xCoord = centerPoint[0];
       double yCoord = centerPoint[1];
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
