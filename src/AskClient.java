import java.net.*;
import java.io.*;

public class AskClient{
    
    public static void main(String [] args) throws Exception{
        Socket socket = new Socket(InetAddress.getLocalHost(), 1234);
        try {
            ObjectOutputStream objectOutputStream = new ObjectOutputStream(socket.getOutputStream());
            ObjectInputStream objectInputStream = new ObjectInputStream(socket.getInputStream());
            objectOutputStream.writeObject(new TestQuery("Does this work?"));
            Object object = objectInputStream.readObject();
            if(object instanceof TestResult){
                System.out.println("Received a Result Query: "+((TestResult) object).test);
            }
            else {
                System.out.println("The Result Object wasn't of the right kind.");
            }
            socket.close();
        }
        catch (IOException e) {
            e.printStackTrace();
        }
    }
}
  
