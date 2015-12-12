using System;

namespace ASKLib {
	[Serializable]
	public class Query {

		public int queryId;
		int userId;
		String deviceId;
	}

	[Serializable]
	public class TestQuery:Query {

		public String test;

		public TestQuery(String inputTest){
			test = inputTest;
		}
	}

	[Serializable]
	public class InsertQuery: Query {

		public AskObject askObject;

		public InsertQuery(AskObject obj) {
			askObject = obj;
		}
	}

	[Serializable]
	public class DeleteQuery: Query {

		public int objectId;

		public DeleteQuery(int id) {
			objectId = id;
		}
	}

	[Serializable]
	public class FetchQuery: Query {

		public float[] centerPoint = new float [2];
		public float[] speedVec = new float [3];
		public float viewAngle;
		public float viewRadius;
		public float compassAngle;

		public float RTT;
		public int[] objectIds;

		public FetchQuery(float[] _centerPoint){
			centerPoint = _centerPoint;
		}
	}
}

//using System;
//using System.Text;
//using Newtonsoft.Json;
//
//namespace AskTest{
//
//	class Tuple2<T,U> {
//
//		public T Item1;
//		public U Item2;
//		public Tuple2(T i1, U i2){
//			Item1 = i1;
//			Item2 = i2;
//		}
//	}
//
//	class PayloadInfo {
//		public int Lenght;
//		public int beginPos[], endPos[];
//
//		public PayloadInfo(){
//
//		}
//	}
//
//	public class Query {
//
//		public int queryId;
//		int userId;
//		String deviceId;
//	}
//
//	class TestQuery:Query {
//
//		public String test;
//
//		public TestQuery(String inputTest){
//			test = inputTest;
//		}
//
//		public static TestQuery Deserialize(byte[] data){
//			int len = BitConverter.ToInt32 (data, 0);
//			int op = BitConverter.ToInt32 (data, 4);
//			byte[] actual = new byte[len-8];
//			Array.ConstrainedCopy (data, 8, actual, 0, len - 8);
//			return JsonConvert.DeserializeObject<TestQuery> (Encoding.ASCII.GetString(actual));
//		}
//
//		public byte[] Serialize(){
//			byte[] self = Encoding.ASCII.GetBytes (JsonConvert.SerializeObject (this));
//			byte[] data = new byte[self.Length+8];
//			byte[] len = BitConverter.GetBytes (self.Length+8);
//			Array.ConstrainedCopy (len, 0, data, 0, len.Length);
//			byte[]  op = BitConverter.GetBytes (0);
//			Array.ConstrainedCopy (op, 0, data, 4, len.Length);
//			Array.ConstrainedCopy (self, 0, data, 8, self.Length);
//			return data;
//		}
//	}
//
//
//	public class InsertQuery: Query {
//
//		public AskObject askObject;
//
//		public InsertQuery(AskObject obj) {
//			askObject = obj;
//		}
//
//
//
//		public byte[] Serialize(){
//			byte[] obj = askObject.objectstream;
//			askObject.objectstream = new byte[0];
//
//			byte[] self = Encoding.ASCII.GetBytes (JsonConvert.SerializeObject (this));
//			byte[] data = new byte[self.Length+8+obj.Length];
//
//			byte[] len = BitConverter.GetBytes (self.Length+8);
//			Array.ConstrainedCopy (len, 0, data, 0, len.Length);
//			byte[]  op = BitConverter.GetBytes (0);
//			Array.ConstrainedCopy (op, 0, data, 4, len.Length);
//
//
//			Array.ConstrainedCopy (self, 0, data, 8, self.Length);
//			return data;
//		}
//	}
//
//	class DeleteQuery: Query {
//
//		public int objectId;
//
//		public DeleteQuery(int id) {
//			objectId = id;
//		}
//
//		public static DeleteQuery Deserialize(byte[] data){
//			int len = BitConverter.ToInt32 (data, 0);
//			int op = BitConverter.ToInt32 (data, 4);
//			byte[] actual = new byte[len-8];
//			Array.ConstrainedCopy (data, 8, actual, 0, len - 8);
//			return JsonConvert.DeserializeObject<DeleteQuery> (Encoding.ASCII.GetString(actual));
//		}
//
//		public byte[] Serialize(){
//			byte[] self = Encoding.ASCII.GetBytes (JsonConvert.SerializeObject (this));
//			byte[] data = new byte[self.Length+8];
//			byte[] len = BitConverter.GetBytes (self.Length+8);
//			Array.ConstrainedCopy (len, 0, data, 0, len.Length);
//			byte[]  op = BitConverter.GetBytes (2);
//			Array.ConstrainedCopy (op, 0, data, 4, len.Length);
//			Array.ConstrainedCopy (self, 0, data, 8, self.Length);
//			return data;
//		}
//	}
//
//	public class FetchQuery: Query {
//
//		public float[] centerPoint = new float [2];
//		public float[] speedVec = new float [3];
//		public float viewAngle;
//		public float viewRadius;
//		public float compassAngle;
//
//		public float RTT;
//		public int[] objectIds;
//
//		public FetchQuery(float[] _centerPoint){
//			centerPoint = _centerPoint;
//		}
//
//		public static FetchQuery Deserialize(byte[] data){
//			int len = BitConverter.ToInt32 (data, 0);
//			int op = BitConverter.ToInt32 (data, 4);
//			byte[] actual = new byte[len-8];
//			Array.ConstrainedCopy (data, 8, actual, 0, len - 8);
//			return JsonConvert.DeserializeObject<FetchQuery> (Encoding.ASCII.GetString(actual));
//		}
//
//		public byte[] Serialize(){
//			byte[] self = Encoding.ASCII.GetBytes (JsonConvert.SerializeObject (this));
//			byte[] data = new byte[self.Length+8];
//			byte[] len = BitConverter.GetBytes (self.Length+8);
//			Array.ConstrainedCopy (len, 0, data, 0, len.Length);
//			byte[]  op = BitConverter.GetBytes (2);
//			Array.ConstrainedCopy (op, 0, data, 4, len.Length);
//			Array.ConstrainedCopy (self, 0, data, 8, self.Length);
//			return data;
//		}
//	}
//}