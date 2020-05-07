using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

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
        public struct Cinema
        {
            public string Name;
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

        public static Page DisplayOrderedSeats(DateTime date, Cinema cinema)
        {
            //fakeData below is an artificial response from database, where each row represents row of seats in the selected cinema
            var fakeData = new List<List<bool>>{
                new List<bool>{ true, true, true, false, true, false},
                new List<bool>{ true, false, false, false},
                new List<bool>{ false, false, true, false},
                new List<bool>{ false, true, true, false},
                new List<bool>{ false, false, false, false, false}
            };
            ///made for possible "prettiness" improvements
            /*
            int maxNumOfSeats = fakeData[0].Count();
            int minNumOfSeats = maxNumOfSeats;
            for (int i = 1; i < fakeData.Count(); i++)
            {
                if (fakeData[i].Count() > maxNumOfSeats) maxNumOfSeats = fakeData[i].Count();
                if (fakeData[i].Count() < minNumOfSeats) minNumOfSeats = fakeData[i].Count();
            }
            */
            return FormOrderedSeatsPage(fakeData);
        }
        private static Page FormOrderedSeatsPage(List<List<bool>> seatsData)
        {
            Page page = new Page();
            Grid grid = new Grid();
            for(int i = 0; i<seatsData.Count(); i++)
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Margin = new Thickness(0, i * 20, 0, 0);
                stackPanel.Orientation = Orientation.Horizontal;
                for(int j = 0; j<seatsData[i].Count(); j++)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Margin = new Thickness(10, 5, 10, 5);
                    checkBox.IsChecked = seatsData[i][j];
                    checkBox.IsEnabled = false;
                    stackPanel.Children.Add(checkBox);
                }
                grid.Children.Add(stackPanel);
            }
            page.Content = grid;
            return page;
        }
    }
}
