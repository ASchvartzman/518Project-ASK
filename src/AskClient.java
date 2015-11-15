import java.net.*;
import java.io.*;
import java.util.HashMap;

public class AskClient{

    public static HashMap<Integer, AskObject> idMap = new HashMap<>();

    static void Handler(Object object, Object objectToSend){
        if(object instanceof TestResult && objectToSend instanceof TestQuery)
            System.out.println("Received a Test Result: "+((TestResult) object).test);
        else if(object instanceof BoolResult && objectToSend instanceof DeleteQuery)
            System.out.println("Received a Bool Result: "+((BoolResult) object).bool);
        else if(object instanceof BoolIntResult && objectToSend instanceof InsertQuery)
            System.out.println("Received a BoolInt Result: "+((BoolIntResult) object).integer+" "+((BoolIntResult) object).bool);
        else{
            System.out.println("Received an incorrect result.");
        }
    }

    static void ServerQuery(Object objectToSend){
        try {
            Socket socket = new Socket(InetAddress.getLocalHost(), 1234);
            ObjectOutputStream objectOutputStream = new ObjectOutputStream(socket.getOutputStream());
            objectOutputStream.writeObject(objectToSend);
            ObjectInputStream objectInputStream = new ObjectInputStream(socket.getInputStream());
            Object object = objectInputStream.readObject();
            Handler(object, objectToSend);
            objectInputStream.close();
            objectOutputStream.close();
            socket.close();
        }
        catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void main(String [] args) throws Exception{
        System.out.println("Why? Hello there, good sir!");
        System.out.println("Isn't it a lovely day?");

        BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(System.in));
        String userQuery = bufferedReader.readLine();
        while(!userQuery.equals("Bye")) {
            if(userQuery.equals("Test"))
                ServerQuery(new TestQuery("Does this work?"));
            else if(userQuery.equals("Cube"))
                ServerQuery(new InsertQuery(new AskCube(1, 0, 0, 0, 0, 2)));
            else if(userQuery.equals("Delete"))
                ServerQuery(new DeleteQuery(0));
            userQuery = bufferedReader.readLine();
        }
    }
}
  
