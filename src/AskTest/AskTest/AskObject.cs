using System;

namespace AskTest{
	public class AskObject 
	{
		public byte[] objectstream;
		public int userId;
		public int targetId;
		public float[] position=new float[2]; 
		public int objectId;

		public AskObject(float[] Coord, byte[] obj, int userID, int objectID, int targetID){
			objectstream = obj;
			userId = userID;
			position=Coord;
			objectId=objectID;
			targetId=targetID;
		}

		/** Get the X coordinate of the center. */
		public float getX(){
			return position[0]; 
		}

		/** Get the Y coordinate for the center. */
		public float getY(){
			return position[1]; 
		}
	}
}