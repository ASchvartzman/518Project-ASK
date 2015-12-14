using System;

namespace ASKExpLib  {
	[Serializable]
	public class AskObject 
	{
		public byte[] objectstream;
		public int userId;
		public int targetId;
		public float[] position=new float[2]; 
		public int objectId;

		public AskObject(float[] Coord, int user, int target, byte[] obj, int objID){
			objectstream = obj;
			position=Coord;
			userId = user;
			targetId = target;
			objectId = objID;
		}
			
		public float getX(){
			return position[0]; 
		}
			
		public float getY(){
			return position[1]; 
		}
	}
}