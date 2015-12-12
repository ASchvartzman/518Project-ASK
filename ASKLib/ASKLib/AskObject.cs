using System;

namespace ASKLib {
	[Serializable]
	public class AskObject 
	{
		public byte[] objectstream;
		public int userId;
		public int targetId;
		public float[] position=new float[2]; 
		public int objectId;

		public AskObject(float[] Coord, byte[] obj){
			objectstream = obj;
			position=Coord;
		}
			
		public float getX(){
			return position[0]; 
		}
			
		public float getY(){
			return position[1]; 
		}
	}
}