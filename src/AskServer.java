import java.net.*;
import java.io.*;

public class AskServer extends Thread {

    Socket socket;

    public AskServer(Socket inputSocket){
        socket = inputSocket;
    }
    
    public void run(){
        try{
            ObjectInputStream objectInputStream = new ObjectInputStream(socket.getInputStream());
            ObjectOutputStream objectOutputStream = new ObjectOutputStream(socket.getOutputStream());
            Object object = objectInputStream.readObject();
            if(object instanceof TestQuery){
                System.out.println("Received a Test Query: "+((TestQuery) object).test);
                objectOutputStream.writeObject(new TestResult("It does"));
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
