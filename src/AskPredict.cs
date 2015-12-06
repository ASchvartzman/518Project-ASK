// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
// x y vx vy \theta dtheta list of objectIDs that you already have
/**
 * Class that performs predictions based on sensor hints (location, speed).
 * Prediction is split into translational and rotational updates.
 * This will return points for the server to query.
 *
 * Moreover, we also implement a function that can be used as an oracle to
 * take out false positives introduced due to the aggressive querying.
 */
//Strategy here is to send the mappings in a fixed radius whose center is predicted using vx, vy

public class AskPredict {
	
	double[] centerPoint = new double[2];
	double[] speedVec = new double[3];
	//double viewAngle;
	double viewRadius;
	//double compassAngle;
	double RTT;
	
	public AskPredict(FetchQuery fetchQuery){
		centerPoint = fetchQuery.centerPoint;
		//viewAngle = fetchQuery.viewAngle;
		speedVec = fetchQuery.speedVec;
		//compassAngle = fetchQuery.compassAngle;
		viewRadius = fetchQuery.viewRadius;
		RTT = fetchQuery.RTT;
		objectIds=fetchQuery.objectIds;
		result.queryId=fetchQuery.queryId;
	}
	
	public double[] PredictRotation(){
		double xCoord = centerPoint[0];
		double yCoord = centerPoint[1];
		double compassChange=RTT*speedVec[2];
		double[] queryPoints = new double [4];
		queryPoints[0] = xCoord - viewRadius*Math.Sin(viewAngle/2)*Math.Cos(compassChange); 
		queryPoints[1] = yCoord - viewRadius*Math.Cos(viewAngle/2)*Math.Sin(compassChange) - viewRadius*Math.Sin(viewAngle/2)*Math.Sin(compassChange); 
		queryPoints[2] = xCoord - viewRadius*Math.Cos(viewAngle/2)*Math.Sin(compassChange) - viewRadius*Math.Sin(viewAngle/2)*Math.Sin(compassChange); 
		queryPoints[3] = yCoord - viewRadius*Math.Sin(viewAngle/2)*Math.Cos(compassChange);        
		return queryPoints; 
	}
	
	public double[] PredictTranslation(){
		double xCoord = centerPoint[0];
		double yCoord = centerPoint[1];
		double xSpeed = speedVec[0];
		double ySpeed = speedVec[1];
		double[] queryPoints = new double [4];
		queryPoints[0] = xCoord + RTT*xSpeed-viewRadius*Math.Sin(viewAngle/2); 
		queryPoints[1] = yCoord + RTT*ySpeed;
		queryPoints[2] = xCoord + RTT*xSpeed+viewRadius*Math.Sin(viewAngle/2); 
		queryPoints[3] = yCoord + RTT*ySpeed+viewRadius*Math.Cos(viewAngle/2);
		return queryPoints; 
	}
	// Here compassAngle I'm assuming to be the angle of horizontal 
	// axis with rectangle covering vision
	public double[] PredictTotal(){
		double xCoord = centerPoint[0]+RTT*speedVec[0];
		double yCoord = centerPoint[1]+RTT*speedVec[1];
		// double cAngle=compassAngle+RTT*speedVec[2];
		// double x=viewRadius;
		// double y=2*viewRadius*Math.Tan(viewAngle/2);
		double[] queryPoints = new double [2];
		// works for angles less than 90 I think 
		// queryPoints[0] = xCoord - x*Math.Sin(cAngle)-y*Math.Cos(cAngle)/2;
		// queryPoints[1] = yCoord - y*Math.Sin(cAngle)/2; 
		// queryPoints[2] = xCoord +y*Math.Cos(cAngle)/2; 
		// queryPoints[3] = yCoord +x*Math.Cos(cAngle)+y*Math.Sin(cAngle)/2;
		queryPoints[0]=xCoord;
		queryPoints[1]=yCoord;
		return queryPoints; 
		
	}
	public Dictionary<double[], double[]> TotalPredict(){
		return new Dictionary<double[],double[] >(new double[]{0, 0}, new double[]{0,0});
	}
	
	public Boolean Separator(double[] candidatePoint){
		double candidateX = candidatePoint[0];
		double candidateY = candidatePoint[1];
		double slope = (candidateY-centerPoint[1])/(candidateX-centerPoint[0]);
		return (Math.Abs(slope) > Math.Tan(90-viewAngle/2));
	}
}
