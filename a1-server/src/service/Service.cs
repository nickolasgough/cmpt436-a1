using System.Collections.Generic;

namespace service
{
    public class Service
    {

        private Dictionary<string, string> users;

        private struct Room {
            public int count;
            public List<string> messages;

            public Room(int count, List<string> messages) {
                this.count = count;
                this.messages = messages;
            }
        }

        private Dictionary<string, Room> rooms;

        public Service() {
            this.users = new Dictionary<string, string>();
            this.rooms = new Dictionary<string, Room>();
        }

        public bool CreateUser(string user) {
            string room = null;

            if (!this.users.TryGetValue(user, out room)) {
                this.users.Add(user, null);
                return true;
            }

            return false;
        }

        public bool DeleteUser(string user) {
            string room = null;

            if (!this.users.TryGetValue(user, out room)) {
                return false;
            }

            this.users.Remove(user);
            
            return true;
        }

        public bool CreateRoom(string room) {
            Room tempR;

            if (!this.rooms.TryGetValue(room, out tempR)) {
                this.rooms.Add(room, new Room(0, new List<string>()));
                return true;
            }

            return true;
        }

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
