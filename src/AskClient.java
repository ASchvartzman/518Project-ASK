import java.net.*;
import java.awt.geom.Point2D;
import java.io.*;

public class AskClient{
    public static void main(String [] args){
        Socket client; 
        PrintWriter out; 
        BufferedReader in; 
        try{
            client = new Socket ("10.9.252.67", 1234); 
            out = new PrintWriter(client.getOutputStream(), true); 
            in = new BufferedReader(new InputStreamReader(client.getInputStream()));
            System.out.println(in.readLine());
            client.close();
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
	
