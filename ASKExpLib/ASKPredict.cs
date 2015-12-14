using System;

namespace ASKExpLib 
{
	public class AskPredict {

		public float[] centerPoint = new float[2];
		public float[] speedVec = new float[3];
		public float viewRadius;
		public float RTT;

		public AskPredict(FetchQuery fetchQuery){
			centerPoint = fetchQuery.centerPoint;
			speedVec = fetchQuery.speedVec;
			viewRadius = fetchQuery.viewRadius;
			RTT = fetchQuery.RTT;
		}


		// Here compassAngle I'm assuming to be the angle of horizontal 
		// axis with rectangle covering vision
		public float[] PredictTotal(){
			float xCoord = centerPoint[0]+RTT*speedVec[0];
			float yCoord = centerPoint[1]+RTT*speedVec[1];
			float[] queryPoints = new float [2];
			queryPoints[0]=xCoord;
			queryPoints[1]=yCoord;
			return queryPoints; 

		}

	}
}

