import java.net.*;
import java.io.*;
import java.util.*;
import java.util.concurrent.atomic.AtomicBoolean;

import net.sf.javaml.core.kdtree.*;

public class AskServer extends Thread {

    Socket socket;
    double maxRadius = 1.0;
    public static KDTree kdTree;
    public static AtomicBoolean engaged;
    public static HashMap<Integer, AskObject> idMap;
    public static int maxObjectId;

    public AskServer(Socket inputSocket){
        socket = inputSocket;
    }

    AbstractMap.SimpleEntry<Boolean, Integer> InsertObject(InsertQuery insertQuery){
        double x = insertQuery.askObject.getX();
        double y = insertQuery.askObject.getY();
        double radius = insertQuery.askObject.getRadius();
        double r2 = radius+maxRadius;
        while(!engaged.compareAndSet(false, true));
        try {
            Object[] neighbors = kdTree.range(new double[]{x-r2, y-r2}, new double[]{x+r2, y+r2});
            for(int i = 0; i < neighbors.length; i++){
                AskObject neighbor = (AskObject) neighbors[i];
                double distance = Math.sqrt( Math.pow(x-neighbor.getX(), 2) + Math.pow(y-neighbor.getY(), 2) );
                if(distance < neighbor.getRadius() + radius)
                    return new AbstractMap.SimpleEntry<>(false, -1);
            }
            insertQuery.askObject.objectId = maxObjectId;
            kdTree.insert(new double[]{x, y}, insertQuery.askObject);
            idMap.put(maxObjectId++, insertQuery.askObject);
        }
        catch (Exception e){
            e.printStackTrace();
            return new AbstractMap.SimpleEntry<>(false, -2);
        }
        engaged.set(false);
        return new AbstractMap.SimpleEntry<>(true, insertQuery.askObject.objectId);
    }

    boolean DeleteObject(DeleteQuery deleteQuery){
        if(!idMap.containsKey(deleteQuery.queryId))
            return false;
        AskObject askObject = idMap.get(deleteQuery.queryId);
        try{
            if (kdTree.search(new double[]{askObject.getX(), askObject.getY()}) == null)
                return false;
            kdTree.delete(new double[]{askObject.getX(), askObject.getY()});
            idMap.remove(deleteQuery.queryId);
        }
        catch (Exception e){
            e.printStackTrace();
            return false;
        }
        return true;
    }

    Object Handle(Object object){
        if(object instanceof TestQuery){
            System.out.println("Received a Test Query: "+((TestQuery) object).test);
            return new TestResult("Indeed!");
        }
        else if(object instanceof InsertQuery){
            System.out.println("Received an Insert Query.");
            AbstractMap.SimpleEntry<Boolean, Integer> result = InsertObject((InsertQuery) object);
            return new BoolIntResult(result.getKey(), result.getValue());
        }
        else if(object instanceof DeleteQuery){
            System.out.println("Received an Delete Query.");
            return new BoolResult(DeleteObject((DeleteQuery) object));
        }
        else if(object instanceof FetchQuery){
            System.out.println("Received an Fetch Query.");
            return new TestResult("Not implemented yet.");
        }
        else {
            System.out.println("The Query Object wasn't of the right kind.");
        }
        return new TestResult("Don't know what to do?");
    }
    
    public void run(){
        try{
            ObjectOutputStream objectOutputStream = new ObjectOutputStream(socket.getOutputStream());
            ObjectInputStream objectInputStream = new ObjectInputStream(socket.getInputStream());
            Object object = objectInputStream.readObject();
            objectOutputStream.writeObject(Handle(object));
            objectInputStream.close();
            objectOutputStream.close();
            socket.close();
        }
        catch (Exception e){
            e.printStackTrace();
        }
    }
    
    public static void main(String [] args){
        engaged = new AtomicBoolean(false);
        kdTree = new KDTree(2);
        idMap = new HashMap<>();
        maxObjectId = 0;
        try {
            ServerSocket serverSocket = new ServerSocket(1234);
            while(true){
                Socket socket = serverSocket.accept();
                AskServer askServer = new AskServer(socket);
                askServer.start();
            }
        }
        catch (IOException e){
            e.printStackTrace();
        }
    }
}