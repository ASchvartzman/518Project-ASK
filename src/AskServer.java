import java.net.*;
import java.io.*;
import java.util.concurrent.atomic.AtomicBoolean;

import net.sf.javaml.core.kdtree.*;

public class AskServer extends Thread {

    Socket socket;
    public static KDTree kdTree;
    public static AtomicBoolean engaged;

    public AskServer(Socket inputSocket){
        socket = inputSocket;
    }
    
    public void run(){
        try{
            ObjectOutputStream objectOutputStream = new ObjectOutputStream(socket.getOutputStream());
            ObjectInputStream objectInputStream = new ObjectInputStream(socket.getInputStream());
            Object object = objectInputStream.readObject();
            if(object instanceof TestQuery){
                System.out.println("Received a Test Query: "+((TestQuery) object).test);
                while(!engaged.compareAndSet(false, true));
                Object answer = kdTree.search(new double[]{1,1});
                if(answer == null)
                    answer = "It clearly doesn't.";
                objectOutputStream.writeObject(new TestResult(answer.toString()));
                engaged.set(false);
            }
            else {
                System.out.println("The Query Object wasn't of the right kind.");
            }
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
        try {
            kdTree.insert(new double[]{1, 1}, 42);
        }
        catch (Exception e){
            e.printStackTrace();
        }

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