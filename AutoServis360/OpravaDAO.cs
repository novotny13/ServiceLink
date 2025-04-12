using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AutoServis360
{
    public class RepairDAO
    {
        public List<Oprava> GetRepairs()
        {
            List<Oprava> repairs = new List<Oprava>();
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"SELECT RepairID, CarID, MechanicID, CustomerID, 
                        ProblemDescription, RepairDescription, TotalCost, 
                        RepairDate, Status FROM Repairs";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        repairs.Add(new Oprava(
                            id: reader.GetInt32(0), // RepairID (povinné)
                            vozidloId: reader.GetInt32(1), // CarID (povinné)
                            mechanikId: reader.IsDBNull(2) ? 0 : reader.GetInt32(2), // MechanicID (NULL -> 0)
                            zakaznikId: reader.GetInt32(3), // CustomerID (povinné)
                            popisProblemu: reader.IsDBNull(4) ? "" : reader.GetString(4), // ProblemDescription (NULL -> "")
                            popisOpravy: reader.IsDBNull(5) ? "" : reader.GetString(5), // RepairDescription (NULL -> "")
                            cena: reader.IsDBNull(6) ? 0 : reader.GetDecimal(6), // TotalCost (NULL -> 0)
                            datumDokonceni: reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7), // RepairDate (může být NULL)
                            status: reader.IsDBNull(8) ? "" : reader.GetString(8) // Status (NULL -> "")
                        ));
                    }
                }
            }
            return repairs;
        }
        public string GetProblemDescription(int carId)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string sql = "SELECT TOP 1 ProblemDescription FROM Repairs WHERE CarID = @CarID ORDER BY RepairDate DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CarID", carId);
                    return cmd.ExecuteScalar()?.ToString() ?? "Popis problému není k dispozici";
                }
            }
        }
        public string GetProblemDescriptionForCar(string licensePlate)
        {
            string description = "Popis problému není k dispozici";
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"SELECT TOP 1 r.ProblemDescription 
                                FROM Repairs r
                                JOIN Cars c ON r.CarID = c.CarID
                                WHERE c.LicensePlate = @LicensePlate
                                AND r.Status IN ('Pending', 'In Progress')
                                ORDER BY r.RepairDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LicensePlate", licensePlate);
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        description = result.ToString();
                    }
                }
            }
            return description;
        }


        public bool CreateRepair(int carId, int customerId, string problemDescription)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO Repairs (CarID, CustomerID, ProblemDescription, Status) 
                       VALUES (@CarID, @CustomerID, @ProblemDescription, 'Pending')";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CarID", carId);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@ProblemDescription", problemDescription);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public void InsertRepair(Oprava oprava)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Repairs (CarID, MechanicID, CustomerID, 
                                ProblemDescription, RepairDescription, TotalCost, RepairDate, Status) 
                                VALUES (@CarID, @MechanicID, @CustomerID, @ProblemDescription, 
                                @RepairDescription, @TotalCost, @RepairDate, @Status)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CarID", oprava.VozidloId);
                    cmd.Parameters.AddWithValue("@MechanicID", (object)oprava.MechanikId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustomerID", oprava.ZakaznikId);
                    cmd.Parameters.AddWithValue("@ProblemDescription", oprava.PopisProblemu);
                    cmd.Parameters.AddWithValue("@RepairDescription", oprava.PopisOpravy);
                    cmd.Parameters.AddWithValue("@TotalCost", oprava.Cena);
                    cmd.Parameters.AddWithValue("@RepairDate", oprava.DatumDokonceni ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", oprava.Status);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteRepair(int repairID)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = "DELETE FROM Repairs WHERE RepairID = @RepairID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RepairID", repairID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void SaveRepair(int repairID, string newDes, int cost)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = "UPDATE Repairs SET RepairDescription = @newDes, TotalCost  = @cost WHERE RepairID = @RepairID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Cost", cost);
                    cmd.Parameters.AddWithValue("@newDes", newDes);
                    cmd.Parameters.AddWithValue("@RepairID", repairID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateRepairStatus(int repairID, string newStatus)
        {
            using (var conn = DatabaseConnection.GetInstance().CreateConnection())
            {
                conn.Open();
                string query = "UPDATE Repairs SET Status = @Status WHERE RepairID = @RepairID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", newStatus);
                    cmd.Parameters.AddWithValue("@RepairID", repairID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}