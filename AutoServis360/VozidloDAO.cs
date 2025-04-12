using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AutoServis360
{
    public class VozidloDAO
    {
        // Získání všech aut
        public List<Vozidlo> GetCars()
        {
            List<Vozidlo> cars = new List<Vozidlo>();
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"SELECT CarID, CustomerID, Brand, Model, LicensePlate, YearOfManufacture FROM Cars";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cars.Add(new Vozidlo(
                            id: reader.GetInt32(0),
                            majitelId: reader.GetInt32(1),
                            znacka: reader.GetString(2),
                            model: reader.GetString(3),
                            spz: reader.GetString(4), // Odpovídá "LicensePlate"
                            rokVyroby: reader.GetInt32(5)
                        ));
                    }
                }
            }
            return cars;
        }
        public int GetCarIdBySPZ(string spz)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string sql = "SELECT CarID FROM Cars WHERE LicensePlate = @SPZ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SPZ", spz);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }
        // Získání auta podle SPZ
        public Vozidlo GetCarByLicensePlate(string licensePlate)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"SELECT CarID, CustomerID, Brand, Model, LicensePlate, YearOfManufacture FROM Cars WHERE LicensePlate = @LicensePlate";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LicensePlate", licensePlate);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Vozidlo(
                                id: reader.GetInt32(0),
                                majitelId: reader.GetInt32(1),
                                znacka: reader.GetString(2),
                                model: reader.GetString(3),
                                spz: reader.GetString(4), // Odpovídá "LicensePlate"
                                rokVyroby: reader.GetInt32(5)
                            );
                        }
                    }
                }
            }
            return null;
        }

        public Vozidlo GetCarByID(int ID)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"SELECT CarID, CustomerID, Brand, Model, LicensePlate, YearOfManufacture FROM Cars WHERE CarID = @ID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Vozidlo(
                                id: reader.GetInt32(0),
                                majitelId: reader.GetInt32(1),
                                znacka: reader.GetString(2),
                                model: reader.GetString(3),
                                spz: reader.GetString(4), // Odpovídá "LicensePlate"
                                rokVyroby: reader.GetInt32(5)
                            );
                        }
                    }
                }
            }
            return null;
        }
        // Vložení nového auta
        public bool InsertCar(Vozidlo auto)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Cars (CustomerID, Brand, Model, LicensePlate, YearOfManufacture)
                                 VALUES (@CustomerID, @Brand, @Model, @LicensePlate, @YearOfManufacture)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", auto.MajitelId);
                    cmd.Parameters.AddWithValue("@Brand", auto.Znacka);
                    cmd.Parameters.AddWithValue("@Model", auto.Model);
                    cmd.Parameters.AddWithValue("@LicensePlate", auto.SPZ);
                    cmd.Parameters.AddWithValue("@YearOfManufacture", auto.RokVyroby);

                    return cmd.ExecuteNonQuery() > 0; // Vrací true, pokud se vloží alespoň jeden řádek
                }
            }
        }
        public bool CarExists(string spz)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"SELECT COUNT(*) FROM Cars WHERE LicensePlate = @LicensePlate";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LicensePlate", spz);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        // Smazání auta podle ID
        public bool DeleteCar(int carId)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = "DELETE FROM Cars WHERE CarID = @CarID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CarID", carId);
                    return cmd.ExecuteNonQuery() > 0; // Vrací true, pokud se smazalo alespoň jedno auto
                }
            }
        }
    }
}
