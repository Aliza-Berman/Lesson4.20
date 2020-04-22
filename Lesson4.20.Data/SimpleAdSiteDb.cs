using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Lesson4._20.Data
{
    public class SimpleAdSiteDb
    {
        private string _connectionString;
        public SimpleAdSiteDb(string connectionString)
        {
            _connectionString = connectionString;

        }
        public void NewAd(Ad ad)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                
                command.CommandText = "INSERT INTO Ad (Offer, Name, PhoneNumber, Date,UserId) " +
                                      "VALUES (@offer, @name, @phone, GETDATE(),@userId) SELECT SCOPE_IDENTITY()";

                command.Parameters.AddWithValue("@offer", ad.Offer);
                object name = ad.Name;
                if (name == null)
                {
                    name = DBNull.Value;
                }
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@phone", ad.PhoneNumber);
                command.Parameters.AddWithValue("@userId", ad.UserId);
                connection.Open();
                ad.Id = (int)(decimal)command.ExecuteScalar();
            }
        }
        public List<Ad> GetAds()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Ad ORDER BY Date DESC";
                connection.Open();
                List<Ad> ads = new List<Ad>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ads.Add(new Ad
                    {
                        Name = (string)reader["Name"],
                        Offer = (string)reader["Offer"],
                        Date = (DateTime)reader["Date"],
                        PhoneNumber = (string)reader["PhoneNumber"],
                        UserId = (int)reader["UserId"]
                    });
                }

                return ads;
            }
        }
        public void AddUser(User user, string password)
        {
            string passHash = BCrypt.Net.BCrypt.HashPassword(password);
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Users(Name,Email,PasswordHash)" +
                                "VALUES(@name,@email,@passHash) SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@passHash", passHash);
                connection.Open();
                user.Id = (int)(decimal)cmd.ExecuteScalar();

            }
        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool IsValidPass = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (IsValidPass)
            {
                return user;
            }
            return null;

        }
        public User GetByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Users WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new User
                {
                    Id = (int)reader["Id"],
                    Email = (string)reader["Email"],
                    Name = (string)reader["Name"],
                    PasswordHash = (string)reader["PasswordHash"]
                };
            }
        }
        public List<Ad> GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Ad  " +
                                      "WHERE UserId = @id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var ads = new List<Ad>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                { 
                    ads.Add(new Ad

                    {
                        Name = (string)reader["Name"],
                        Offer = (string)reader["Offer"],
                        Date = (DateTime)reader["Date"],
                        PhoneNumber = (string)reader["PhoneNumber"],
                        UserId = (int)reader["UserId"]
                    });
                }
                return ads;
            }
        }
        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Ad WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        
        
    }
}
