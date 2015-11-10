import java.net.*;
import java.io.*;

public class AskServer extends Thread {
    
    ServerSocket server; 
    
    public AskServer() throws Exception
    {
        server = new ServerSocket(1234); 
        server.setSoTimeout(10000);
    }
    
    public void run(){
        while(true){
            PrintWriter out; 
            BufferedReader in; 
            Socket client; 
            try{
                client = server.accept(); 
                out = new PrintWriter(client.getOutputStream(), true); 
                in = new BufferedReader(new InputStreamReader(client.getInputStream()));  
                out.println("Hello Darkness my old friend!");
                client.close();
            }
            catch(Exception e){
                System.out.println("An error has occured!"); 
            }
        }
    }
    
    public String handleRequest(String input) {
        String[] parsedRequest = input.split(" ");
        if (parsedRequest[0].equals("insert")) {
            // then call some function here which whill insert points
            return "Inserted a point!";
        }
        else if (parsedRequest.equals("delete")){
            // then call some function here which will delete a point
            return "Delete a point!";
        }
        else if (parsedRequest.equals("query")){
            // then call some function here which will query a point
            return "Queried a point!";
        }
        else {
            return "Error!?";
        }
    }
    
    public static void main(String [] args){
       
        try{ 
            Thread t = new AskServer();
            t.start();
        }
        catch(Exception e){
            System.out.println("Error! AAAAAAh!"); 
        }
        ServerSocket server; 
    }
}
