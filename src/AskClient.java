import java.net.*;
import java.io.*;

public class AskClient{
    
    public static void main(String [] args) throws Exception{
        Socket socket = new Socket(InetAddress.getLocalHost(), 1234);
        try {
            ObjectOutputStream objectOutputStream = new ObjectOutputStream(socket.getOutputStream());
            objectOutputStream.writeObject(new TestQuery("Does this work?"));
            ObjectInputStream objectInputStream = new ObjectInputStream(socket.getInputStream());
            Object object = objectInputStream.readObject();
            if(object instanceof TestResult){
                System.out.println("Received a Test Result: "+((TestResult) object).test);
            }
            else {
                System.out.println("The Result Object wasn't of the right kind.");
            }
            objectInputStream.close();
            objectOutputStream.close();
            socket.close();
        }
        catch (IOException e) {
            e.printStackTrace();
        }
    }
}
  
