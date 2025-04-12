using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace AutoServis360
{
    public class UserDAO
    {
        public User AuthenticateUser(string username, string password)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = "SELECT UserID, Username, PasswordHash, Role FROM Users WHERE Username = @Username";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())  // Pokud najde uživatele
                        {
                            string storedHashedPassword = reader.GetString(2);
                            int userId = reader.GetInt32(0);
                            string dbUsername = reader.GetString(1);
                            
                            string role = reader.GetString(3);

                            

                            if (VerifyPassword(password, storedHashedPassword))
                            {
                                Console.WriteLine("Heslo správné, přihlašuji...");
                                return new User(
                                    reader.GetInt32(0),
                                    reader.GetString(1),
                                    storedHashedPassword,
                                    reader.GetString(3)
                                );
                            }
                            else
                            {
                                Console.WriteLine("Špatné heslo!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Uživatel nenalezen!");
                        }
                    }
                }
            }
            return null;
        }

       

        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputPassword);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashedInputPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                Console.WriteLine("Zadané heslo: " + inputPassword);
                Console.WriteLine("Očekávaný hash: " + storedHashedPassword);
                Console.WriteLine("Vypočítaný hash: " + hashedInputPassword);

                return hashedInputPassword == storedHashedPassword;
            }
        }

    }
}
