package service;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.PrintStream;
import java.net.Socket;
import java.util.ArrayList;

/*
Service class for establish connections
Commands are sent to the server to be handled
 */
public class Service {

    /* Store all necessary data */
    private Socket socket;
    private PrintStream printStream;
    private BufferedReader bufferedReader;

    private String userId;
    private String room;

    /* Instantiate the service and initialize the server connection */
    public Service() {
        this.userId = null;

        String ipAddress = "127.0.0.1";
        int port = 8080;

        try {
            this.socket = new Socket(ipAddress, port);
        } catch (Exception e) {
            System.err.println("error: socket - failed to establish a connection with the server");
        }

        try {
            this.bufferedReader = new BufferedReader(new InputStreamReader(this.socket.getInputStream()));
            this.printStream = new PrintStream(this.socket.getOutputStream());
        } catch (Exception e) {
            System.err.println("error: socket - failed to establish input and output connections with the server");
        }
    }

    /* Return the user's current room */
    public String currentRoom() {
        return this.room;
    }

    /* Create a new user on the chat room server */
    public boolean CreateUser(String user) {
        this.printStream.println("user");
        this.printStream.println(user);

        String response;
        boolean success;
        try {
            response = this.bufferedReader.readLine();
            success = Boolean.parseBoolean(response);
            this.userId = success ? user : null;
            return success;
        } catch (Exception e) {
            System.err.println("error: user - failed to receive response from server");
        }

        return false;
    }

    /* Create a new chat room on the chat room server */
    public boolean CreateRoom(String room) {
        this.printStream.println("create");
        this.printStream.println(room);

        boolean success;
        String response;
        try {
            response = this.bufferedReader.readLine();
            success = Boolean.parseBoolean(response);
            return success;
        } catch (Exception e) {
            System.err.println("error: create - failed to receive response from server");
        }

        return false;
    }

    /* Join a chat room on the chat room server and retrieve all existing messages */
    public ArrayList<String> JoinRoom(String room) {
        this.printStream.println("join");
        this.printStream.println(room);
        this.printStream.println(this.userId);

        String response;
        boolean success;
        int numMsgs;
        ArrayList<String> messages;
        try {
            response = this.bufferedReader.readLine();
            success = Boolean.parseBoolean(response);
            if (!success) {
                return null;
            }

            numMsgs = Integer.parseInt(this.bufferedReader.readLine());
            if (numMsgs < 0) {
                return null;
            }

            messages = new ArrayList<String>();
            for (int c = 0; c < numMsgs; c += 1) {
                messages.add(this.bufferedReader.readLine());
            }

            this.room = room;
            return messages;
        } catch (Exception e) {
            System.err.println("error: join - failed to receive response from server");
        }

        return null;
    }

    /* Leave the user's current chat room on the chat room server */
    public boolean LeaveRoom() {
        this.printStream.println("leave");
        this.printStream.println(this.userId);

        boolean success;
        try {
            success = Boolean.parseBoolean(this.bufferedReader.readLine());

            if (success) {
                this.room = null;
            }
            return success;
        } catch (Exception e) {
            System.err.println("error: leave - failed to receive response from server");
        }

        return false;
    }

    /* List all existing chat rooms on the chat room server */
    public ArrayList<String> ListRooms() {
        this.printStream.println("list");

        String response;
        boolean success;
        int numRms;
        ArrayList<String> rooms;
        try {
            response = this.bufferedReader.readLine();
            success = Boolean.parseBoolean(response);
            if (!success) {
                return null;
            }

            numRms = Integer.parseInt(this.bufferedReader.readLine());
            if (numRms < 0) {
                return null;
            }

            rooms = new ArrayList<String>();
            for (int c = 0; c < numRms; c += 1) {
                rooms.add(this.bufferedReader.readLine());
            }
            return rooms;
        } catch (Exception e) {
            System.err.println("error: list - failed to receive response from server");
        }
        return null;
    }

    /* Send a message to the user's current chat room on the chat room server */
    public boolean SendMessage(String message) {
        this.printStream.println("send");
        this.printStream.println(this.userId);
        this.printStream.println(message);

        boolean success;
        try {
            success = Boolean.parseBoolean(this.bufferedReader.readLine());
            return success;
        } catch (Exception e) {
            System.err.println("error: send - failed to receive a response from the server");
        }

        return false;
    }

    /* List all the existing messages in the user's current chat room */
    public ArrayList<String> RefreshMessages() {
        this.printStream.println("refresh");
        this.printStream.println(this.userId);

        String response;
        boolean success;
        int numMsgs;
        ArrayList<String> messages;
        try {
            response = this.bufferedReader.readLine();
            success = Boolean.parseBoolean(response);
            if (!success) {
                return null;
            }

            numMsgs = Integer.parseInt(this.bufferedReader.readLine());
            if (numMsgs < 0) {
                return null;
            }

            messages = new ArrayList<String>();
            for (int c = 0; c < numMsgs; c += 1) {
                messages.add(this.bufferedReader.readLine());
            }

            return messages;
        } catch (Exception e) {
            System.err.println("error: refresh - failed to receive response from server");
        }

        return null;
    }

    /* End the current connection with the chat room server */
    public void EndConnection() {
        try {
            this.socket.close();
        } catch (Exception e) {
            System.err.println("error: socket - failed to gracefully close connection");
        }
    }
}
