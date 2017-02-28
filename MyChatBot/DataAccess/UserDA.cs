using MyChatBot.Controllers;
using MyChatBot.DBController;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MyChatBot.DataAccess
{
    public class UserDA
    {
        private DBConnection conn;

        public UserDA()
        {
            conn = new DBConnection();
        }

        public DataTable getAllUsers()
        {
            string query = string.Format("select * from users");
            MySqlParameter[] parameters = new MySqlParameter[0];
            return conn.select(query, parameters);
        }
    }
}