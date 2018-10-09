package client;

import service.Service;

import java.io.ByteArrayInputStream;
import java.util.ArrayList;
import java.util.Scanner;

/*
Client class to handle user interactions on the terminal
*/
public class Client {

    /* Store the service that will make the requests */
    private Service service;

    /* Create the client and instantiate the service */
    public Client() {
        this.service = new Service();
    }

    /* Run the client program */
    public void Run() {
        /* Initial setup */
        String input;
        Scanner scanner = new Scanner(System.in);

        /* Register the user */
        this.print("Please enter your username: ");
        input = scanner.nextLine();
        while (input.isEmpty()) {
            this.print("Please enter a valid username: ");
            input = scanner.nextLine();
        }

        boolean success = this.service.CreateUser(input);
        while (!success) {
            this.println("Failed to create user");

            this.print("Please enter a valid username: ");
            input = scanner.nextLine();
            while (input.isEmpty()) {
                this.print("Please enter a valid username: ");
                input = scanner.nextLine();
            }

            success = this.service.CreateUser(input);
        }
        this.println("User successfully created");

        /* Continuosly accept commands from the user */
        this.print(this.getPrompt());
        input = scanner.nextLine();
        while (!input.equals("quit")) {
            this.handleInput(input);
            this.print(this.getPrompt());
            input = scanner.nextLine();
        }

        /* Close the connection and cleanup */
        this.service.LeaveRoom();
        this.service.EndConnection();
    }

    /* Compute the appropriate display prompt */
    private String getPrompt() {
        String prompt = "What would you like to do? ";
        String room = this.service.currentRoom();

        if (room != null) {
            return prompt + "(" + room + ") ";
        }
        return prompt;
    }

    /* Parse and handle the user's input */
    private void handleInput(String input) {
        Scanner scanner = new Scanner(new ByteArrayInputStream(input.getBytes()));

        /* Retrieve the command */
        String command;
        if (scanner.hasNext()) {
            command = scanner.next().trim();
        } else {
            return;
        }

        /* React to the command, collecting more input when necessary */
        String argument;
        boolean success;
        ArrayList<String> results;
        switch (command) {
            case "list":
                if (scanner.hasNext()) {
                    this.errorln("usage: list");
                    return;
                }

                results = this.service.ListRooms();
                if (results != null) {
                    results.forEach(s -> this.println(s));
                } else {
                    this.errorln("Failed to list the existing rooms");
                }
                break;
            case "create":
                if (!scanner.hasNext()) {
                    this.errorln("usage: create <room>");
                    return;
                }

                argument = scanner.nextLine().trim();
                success = this.service.CreateRoom(argument);
                this.println(success ? "Room successfully created" : "Failed to create room");
                break;
            case "join":
                if (!scanner.hasNext()) {
                    this.errorln("usage: join <room>");
                    return;
                }

                argument = scanner.nextLine().trim();
                results = this.service.JoinRoom(argument);
                if (results != null) {
                    results.forEach(s -> this.println(s));
                } else {
                    this.errorln("Failed to join the room");
                }
                break;
            case "leave":
                if (scanner.hasNext()) {
                    this.errorln("usage: leave");
                    return;
                }

                success = this.service.LeaveRoom();
                this.println(success ? "Room successfully left" : "Failed to leave room");
                break;
            case "send":
                if (!scanner.hasNext()) {
                    this.errorln("usage: send <message>");
                    return;
                }

                argument = scanner.nextLine().trim();
                success = this.service.SendMessage(argument);
                this.println(success ? "Message successfully sent" : "Failed to send message");
                break;
            case "refresh":
                if (scanner.hasNext()) {
                    this.errorln("usage: refresh");
                    return;
                }

                results = this.service.RefreshMessages();
                if (results != null) {
                    results.forEach(s -> this.println(s));
                } else {
                    this.errorln("Failed to refresh the messages");
                }
                break;
            default:
                this.errorln("error: given command is unknown");
                this.println("invoke \"help\" for a list of commands");
        }
    }

    /* Print a message without a newline */
    private void print(String message) {
        System.out.print(message);
    }

    /* Print a new message with a newline */
    private void println(String message) {
        System.out.println(message);
    }

    /* Print an error with a newline */
    private void errorln(String error) {
        System.err.println(error);
    }
}
