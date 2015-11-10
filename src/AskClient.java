import java.net.*;
import java.io.*;

public class AskClient{
    
    public static AskServer server;
    public static Socket client;
    
    public AskClient(Socket MyClient, AskServer MyServer){
        client = MyClient;
        server = MyServer;
    }
    
    public static void main(String [] args) throws Exception{
        
        PrintWriter out; 
        BufferedReader in; 
        client = new Socket ("140.180.189.48", 1234); 
        try{
            out = new PrintWriter(client.getOutputStream(), true); 
            in = new BufferedReader(new InputStreamReader(client.getInputStream()));
            try{
                for (String line =in.readLine(); line!=null; line=in.readLine()) {
                    //Getting null pointer error here. I need to define the server somewhere, 
                    // but i think that it cant be here. 
                    String output = server.handleRequest(line);
                    out.println(output);
                } 
            }
            finally{
                client.close();
            }
        }            
       
        catch (UnknownHostException e){ 
            System.out.println("Unknown host"); 
            System.exit(1); 
            } 
        catch (IOException e) {
            System.out.println("No I/O"); 
            System.exit(1); 
            } 
    }
}
  
