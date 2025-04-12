using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AutoServis360
{
    public class MechanicDAO
    {
        // Získání všech aut, která nejsou přiřazena žádnému mechanikovi
        public List<Vozidlo> GetAvailableCars()
        {
            List<Vozidlo> cars = new List<Vozidlo>();
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string sql = @"SELECT DISTINCT c.CarID, c.CustomerID, c.Brand, c.Model, c.LicensePlate, c.YearOfManufacture
                               FROM Cars c
                               JOIN Repairs r ON c.CarID = r.CarID
                               WHERE r.Status = 'Pending' 
                               AND r.MechanicID IS NULL";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cars.Add(new Vozidlo(
                                id: reader.GetInt32(0),
                                majitelId: reader.GetInt32(1),
                                znacka: reader.GetString(2),
                                model: reader.GetString(3),
                                spz: reader.GetString(4),
                                rokVyroby: reader.GetInt32(5)
                            ));
                        }
                    }
                }
            }
            return cars;
        }


        // Získání všech aut, která jsou přiřazena mechanikovi podle aktivních oprav
        public List<Vozidlo> GetMechanicCars(int mechanicID)
        {
            List<Vozidlo> cars = new List<Vozidlo>();
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string sql = @"SELECT DISTINCT c.CarID, c.CustomerID, c.Brand, c.Model, c.LicensePlate, c.YearOfManufacture
                               FROM Cars c
                               JOIN Repairs r ON c.CarID = r.CarID
                               WHERE r.MechanicID = @mechanicID AND r.Status != 'completed'";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mechanicID", mechanicID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cars.Add(new Vozidlo(
                                id: reader.GetInt32(0),
                                majitelId: reader.GetInt32(1),
                                znacka: reader.GetString(2),
                                model: reader.GetString(3),
                                spz: reader.GetString(4),
                                rokVyroby: reader.GetInt32(5)
                            ));
                        }
                    }
                }
            }
            return cars;
        }

        // Přiřazení auta mechanikovi (vytvoření nové opravy)
        public bool AssignCarToMechanic(int mechanicID, string spz)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string sql = @"UPDATE Repairs 
                              SET MechanicID = @mechanicID, 
                                  Status = 'In Progress'
                              WHERE CarID = (SELECT CarID FROM Cars WHERE LicensePlate = @spz)
                              AND Status = 'Pending'
                              AND MechanicID IS NULL";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mechanicID", mechanicID);
                    cmd.Parameters.AddWithValue("@spz", spz);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public Mechanic GetMechanicByUserId(int userId)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = "SELECT MechanicID, UserID, FirstName, LastName, Specialization FROM Mechanics WHERE UserID = @UserID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Mechanic(
                                reader.GetInt32(0), // MechanicID
                                reader.GetInt32(1), // UserID
                                reader.GetString(2), // FirstName
                                reader.GetString(3), // LastName
                                reader.IsDBNull(4) ? null : reader.GetString(4) // Specialization (nullable)
                            );
                        }
                    }
                }
            }
            return null;
        }
        // Označení opravy jako dokončené
        public bool CompleteRepair(int mechanicID, string spz)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string sql = @"UPDATE Repairs 
                       SET Status = 'Completed',
                           RepairDate = Sysdate()
                       WHERE MechanicID = @mechanicID 
                       AND CarID = (SELECT CarID FROM Cars WHERE LicensePlate = @spz)
                       AND Status = 'In Progress'"; 

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mechanicID", mechanicID);
                    cmd.Parameters.AddWithValue("@spz", spz);

                    int affectedRows = cmd.ExecuteNonQuery();

                    if (affectedRows == 0)
                    {
                        // Logování pro debugování
                        Console.WriteLine($"Žádný záznam nebyl aktualizován. Mechanik: {mechanicID}, SPZ: {spz}");
                    }

                    return affectedRows > 0;
                }
            }
        }


    }
}
