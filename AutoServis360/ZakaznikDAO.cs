using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AutoServis360
{
    public class ZakaznikDAO
    {
        // ✅ Získání všech zákazníků
        public List<Zakaznik> GetAllCustomers()
        {
            List<Zakaznik> customers = new List<Zakaznik>();
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = "SELECT CustomerID, FirstName, LastName, Email, Phone, Address FROM Customers";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new Zakaznik(
                            id: reader.GetInt32(0),
                            jmeno: reader.GetString(1),
                            prijmeni: reader.GetString(2),
                            email: reader["Email"] as string,
                            telefon: reader["Phone"] as string,
                            adresa: reader["Address"] as string
                        ));
                    }
                }
            }
            return customers;
        }

        // ✅ Získání zákazníka podle ID
        public Zakaznik GetCustomerById(int id)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = "SELECT CustomerID, FirstName, LastName, Email, Phone, Address FROM Customers WHERE CustomerID = @CustomerID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Zakaznik(
                                id: reader.GetInt32(0),
                                jmeno: reader.GetString(1),
                                prijmeni: reader.GetString(2),
                                email: reader["Email"] as string,
                                telefon: reader["Phone"] as string,
                                adresa: reader["Address"] as string
                            );
                        }
                    }
                }
            }
            return null;
        }

        // ✅ Vložení nového zákazníka
        public bool InsertCustomer(Zakaznik zakaznik)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Customers (FirstName, LastName, Email, Phone, Address) 
                                 VALUES (@FirstName, @LastName, @Email, @Phone, @Address)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", zakaznik.Jmeno);
                    cmd.Parameters.AddWithValue("@LastName", zakaznik.Prijmeni);
                    cmd.Parameters.AddWithValue("@Email", (object)zakaznik.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", (object)zakaznik.Telefon ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)zakaznik.Adresa ?? DBNull.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ✅ Aktualizace zákazníka
        public bool UpdateCustomer(Zakaznik zakaznik)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"UPDATE Customers 
                                 SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Phone = @Phone, Address = @Address 
                                 WHERE CustomerID = @CustomerID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", zakaznik.Id);
                    cmd.Parameters.AddWithValue("@FirstName", zakaznik.Jmeno);
                    cmd.Parameters.AddWithValue("@LastName", zakaznik.Prijmeni);
                    cmd.Parameters.AddWithValue("@Email", (object)zakaznik.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", (object)zakaznik.Telefon ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)zakaznik.Adresa ?? DBNull.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public int GetCustomerId(Zakaznik zakaznik)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"SELECT CustomerID FROM Customers 
                         WHERE FirstName = @FirstName 
                         AND LastName = @LastName 
                         AND Phone = @Phone";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", zakaznik.Jmeno);
                    cmd.Parameters.AddWithValue("@LastName", zakaznik.Prijmeni);
                    cmd.Parameters.AddWithValue("@Phone", zakaznik.Telefon ?? (object)DBNull.Value);

                    var result = cmd.ExecuteScalar();
                    return result != null ? (int)result : -1; // -1 znamená, že zákazník nebyl nalezen
                }
            }
        }
        public bool CustomerExists(Zakaznik zakaznik)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"SELECT COUNT(*) FROM Customers 
                         WHERE FirstName = @FirstName 
                         AND LastName = @LastName 
                         AND Phone = @Phone";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", zakaznik.Jmeno);
                    cmd.Parameters.AddWithValue("@LastName", zakaznik.Prijmeni);
                    cmd.Parameters.AddWithValue("@Phone", zakaznik.Telefon ?? (object)DBNull.Value);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        // ✅ Smazání zákazníka podle ID
        public bool DeleteCustomer(int id)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = "DELETE FROM Customers WHERE CustomerID = @CustomerID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
