import java.io.*;
import Math; 

abstract class Query implements Serializable {

    int queryId;
    AskObject queryObj;
    double x = queryObj.getX();
    double y = queryObj.getY();
    double radius = queryObj.getRadius();
    double[] center = new double [2];
    center[0] = x;
    center[1] = y;
}

class TestQuery extends Query {

    String test;

    public TestQuery(String inputTest){
        test = inputTest;
    }
}

/**
 * 
 * @author arielschvartzman
 * 
 */

class InsertQuery extends Query {
	public boolean insertQuery(){
	    
	    double[] low = new double [2];
	    low[0] = x-1;
	    low[1] = y-1; 
	    double[] high = new double [2];
	    high[0] = x+1;
	    high[1] = y+1;
	    
		AskObject[] neighbors = (AskObject) kdTree.range(low, high);
		for(int i = 0; i < neighbors.length; i++){
            double neighborX = neighbors[i].getX();
            double neighborY = neighbors[i].getY();
            double neighborR = neighbors[i].getRadius();
            double distance = Math.sqrt(Math.pow(x-neighborX,2)+Math.pow(y-neighborY,2));
		    if(distance < neighborR + radius){
		        return false;
		    }
		}
		
		double[] center = new double [2];
		center[0] = x;
		center[1] = y;
		kdTree.insert(center);
		return true; 
	}
}

class DeleteQuery extends Query {
	public boolean deleteQuery(){
		if (kdTree.searh(center).equals(null)){
			return false; 
		}
		kdTree.delete(queryId);
		return true;
	}
}

class FetchQuery extends Query {

}