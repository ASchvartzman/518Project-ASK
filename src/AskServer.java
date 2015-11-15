import java.net.*;
import java.io.*;
import java.util.*;
import java.util.concurrent.atomic.AtomicBoolean;

import net.sf.javaml.core.kdtree.*;

/** AskClient is the server program. */
public class AskServer extends Thread {

    Socket socket;
    double maxRadius = 1.0;
    /** KDTree kdtree -- The KD Tree stored at the server.
     * Declared public static so that multiple threads share the same object. */
    public static KDTree kdTree;
    /** HashMap(Integer, AskObject) idMap -- The map from object identifier to AskObject stored at the server.
     * Declared public static so that multiple threads share the same object. */
    public static HashMap<Integer, AskObject> idMap;
    /** int maxObjectId -- Stores the least assignable object identifier for a new object.
     * Declared public static so that multiple threads share the same object. */
    public static int maxObjectId;
    /** AtomicBoolean engaged -- Locks state while adding entries to KD Tree, idMap or updating maxObjectId.
     * Declared public static so that multiple threads share the same object. */
    public static AtomicBoolean engaged;

    public AskServer(Socket inputSocket){
        socket = inputSocket;
    }

    /** InsertObject is a server-side function for inserting objects in to the KD Tree.
     *
     * @param insertQuery This is the InsertQuery instance sent by the client.
     * @return A pair of Boolean and Integer, specifying if the operation was successful and the new object identifier.
     * If the operation fails because of presence of overlap, the Integer is set to (-1).
     * If the operation fails because some exception, the Integer is set to (-2).
     */
    AbstractMap.SimpleEntry<Boolean, Integer> InsertObject(InsertQuery insertQuery){
        double x = insertQuery.askObject.getX();
        double y = insertQuery.askObject.getY();
        double radius = insertQuery.askObject.getRadius();
        /** double r2 -- A conservative estimate on the distance between centers that can cause overlap. */
        double r2 = radius+maxRadius;

        /** Block the execution until you get a go-ahead. */
        while(!engaged.compareAndSet(false, true));

        try {
            Object[] neighbors = kdTree.range(new double[]{x-r2, y-r2}, new double[]{x+r2, y+r2});
            for(int i = 0; i < neighbors.length; i++){
                AskObject neighbor = (AskObject) neighbors[i];
                /** If there exists a point which might overlap (determined from globalRadius), reject. */
                double distance = Math.sqrt( Math.pow(x-neighbor.getX(), 2) + Math.pow(y-neighbor.getY(), 2) );
                if(distance < neighbor.getRadius() + radius)
                    return new AbstractMap.SimpleEntry<>(false, -1);
            }
            /** Otherwise, insert. */
            insertQuery.askObject.objectId = maxObjectId;
            kdTree.insert(new double[]{x, y}, insertQuery.askObject);
            idMap.put(maxObjectId++, insertQuery.askObject);
        }
        catch (Exception e){
            e.printStackTrace();
            return new AbstractMap.SimpleEntry<>(false, -2);
        }
        /** Do not forget to hand over the lock. */
        engaged.set(false);
        return new AbstractMap.SimpleEntry<>(true, insertQuery.askObject.objectId);
    }

    /** DeleteObject is a server-side function for deleting objects from the KD Tree.
     *
     * @param deleteQuery This is the DeleteQuery instance sent by the client.
     * @return A Boolean denoting if the operation was successful.
     */
    boolean DeleteObject(DeleteQuery deleteQuery){
        /** Checks if the received queryId is in idMap. */
        // TODO: 11/15/15  (Karan) It might be alright to say 'true'.
        if(!idMap.containsKey(deleteQuery.queryId))
            return false;

        AskObject askObject = idMap.get(deleteQuery.queryId);
        try{
            /** If the object is there, delete it. */
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

    /** Handle is the primary function that processes and categorizes all client requests.
     *
     * @param object Expects an instance of an object inheriting Query.
     * @return An object inheriting Result.
     */
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
            // TODO: 11/15/15   In case of a mismatch, I (Karan) recommend that this should throw an exception.
            System.out.println("The Query Object wasn't of the right kind.");
            return new TestResult("Don't know what to do?");
        }
    }

    /** This is the method that gets invoked on every thread's start().
     * Defers the processing to Handle(), and handles the networking components.
     */
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
            /** Waits to receive a client call. As soon as it gets one, forks a new instance to handle the same. */
            while(true){
                /** The accept() here is blocking. */
                Socket socket = serverSocket.accept();
                AskServer askServer = new AskServer(socket);
                /** Start the thread. */
                askServer.start();
            }
        }
        catch (IOException e){
            e.printStackTrace();
        }
    }
}