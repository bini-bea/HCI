using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace login
{
    public static class SessionManager
    {
        // A static property to store the current user's username or ID
        public static string CurrentUser { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        // Add other properties as needed
    }

}

