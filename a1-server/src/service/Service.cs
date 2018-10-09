using System.Collections.Generic;

namespace service
{
    /*
    Service class for storing and manipulating data.
    Users, chat rooms, and messages are all stored here.
    */
    public class Service
    {

        /* Users and their current chat room */
        private Dictionary<string, string> users;

        /* Structure for storing chat room data */
        private struct Room {
            public int count;
            public List<string> messages;

            public Room(int count, List<string> messages) {
                this.count = count;
                this.messages = messages;
            }
        }

        /* Rooms and their current messages */
        private Dictionary<string, Room> rooms;

        /* Create a service and instantiate the data */
        public Service() {
            this.users = new Dictionary<string, string>();
            this.rooms = new Dictionary<string, Room>();
        }

        /* Create the given user */
        public bool CreateUser(string user) {
            string room = null;

            if (!this.users.TryGetValue(user, out room)) {
                this.users.Add(user, null);
                return true;
            }

            return false;
        }

        /* Delete the give user */
        public bool DeleteUser(string user) {
            string room = null;

            if (!this.users.TryGetValue(user, out room)) {
                return false;
            }

            this.users.Remove(user);
            
            return true;
        }

        /* Create the given room */
        public bool CreateRoom(string room) {
            Room tempR;

            if (!this.rooms.TryGetValue(room, out tempR)) {
                this.rooms.Add(room, new Room(0, new List<string>()));
                return true;
            }

            return true;
        }

        /* The given user tries to join the given chat room */
        public List<string> JoinRoom(string room, string user) {
            string tempR = null;
            Room r;

            if (!this.users.TryGetValue(user, out tempR)) {
                return null;
            }
            if (tempR != null) {
                return null;
            }

            if (!this.rooms.TryGetValue(room, out r)) {
                return null;
            }
            if (r.count >= 10) {
                return null;
            }

            this.users[user] = room;
            r.count += 1;
            this.rooms[room] = r;

            return r.messages;
        }

        /* The given user tries to leave their current chat room */
        public bool LeaveRoom(string user) {
            string room = null;
            Room r;

            if (!this.users.TryGetValue(user, out room)) {
                return false;
            }
            if (room == null) {
                return false;
            }

            if (!this.rooms.TryGetValue(room, out r)) {
                return false;
            }
            if (r.count <= 0) {
                return false;
            }

            this.users[user] = null;
            r.count -= 1;
            this.rooms[room] = r;

            return true;
        }

        /* List all existing chat rooms */
        public List<string> ListRooms() {
            int count = this.rooms.Keys.Count;
            List<string> rooms = new List<string>();
            Room r;

            foreach (string key in this.rooms.Keys) {
                r = this.rooms[key];
                rooms.Add(key + " (" + r.count + "/10)");
            }

            return rooms;
        }

        /* The given user tries to send a message to their current chat room */
        public bool SendMessage(string user, string message) {
            string room = null;
            Room r;

            if (!this.users.TryGetValue(user, out room)) {
                return false;
            }
            if (room == null) {
                return false;
            }

            if (!this.rooms.TryGetValue(room, out r)) {
                return false;
            }

            r.messages.Add(user + " -> " + message);
            this.rooms[room] = r;

            return true;
        }

        /* List all the messages within the user's current chat room */
        public List<string> RefreshMessages(string user) {
            string room = null;
            Room r;

            if (!this.users.TryGetValue(user, out room)) {
                return null;
            }
            if (room == null) {
                return null;
            }

            if (!this.rooms.TryGetValue(room, out r)) {
                return null;
            }

            return r.messages;
        }
    }
}
