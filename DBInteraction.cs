using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Cinema
{
    public static class DBInteraction
    {
        public struct Ticket
        {
            public string ticketId;
            public string filmName;
            public string cinemaName;
            public DateTime filmDate;
            public string cinemaAddress;
            public string seatCode;
        }

        private static string ConnectionString =
            $"Data Source={Properties.Resources.DBServerName};" +
            $"Initial Catalog={Properties.Resources.DBInitialCatalog};"+
            $"User id={Properties.Resources.DBUser};"+
            $"Password={Properties.Resources.DBPassword};"+
            $"Connection Timeout={Properties.Resources.ConnectionTimeout}";
        /// <summary>
        /// Searches for certain user in a database.
        /// </summary>
        /// <param name="username">User's login</param>
        /// <param name="password">User's password</param>
        /// <returns>If user exists then role, else raises an exception</returns>
        public static string LoginCheck(string username, string password)
        {
            string query = $"select Role from User where Login = '{username}' and Password = '{password}'";
            string output = "";
            using(SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)//if user exists
                    {
                        reader.Read();
                        output = reader[0].ToString();//reader can't have more than 1 row due to database's integrity constraints
                    }
                    else
                    {
                        throw new Exception(Properties.Resources.UserNotFoundQueryResponse);
                    }
                }
                return output;
            }
        }
    }
}
