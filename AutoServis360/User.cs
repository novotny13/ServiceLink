using System;

namespace AutoServis360
{
    public class User
    {
        private int id;
        private string username;
        private string password;
        private string position; // Správná privátní proměnná

        public User(int id, string username, string password, string position)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.position = position;
        }

        public int Id { get => id; set => id = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string Position { get => position; set => position = value; } // Opravený getter a setter

        public override string? ToString()
        {
            return "username> " + username + "  Password:  " + Password + "  role  " + position;
        }
    }
}
