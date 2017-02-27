using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MyChatBot.Controllers
{
    public class DBConnection
    {
        MySqlConnection con = new MySqlConnection();
        MySqlDataAdapter adapter;   

        public DBConnection()
        {
        }

        public MySqlConnection openConnection()
        {
            try
            {
                con.ConnectionString = "server=localhost;user id=root;database=thiepcuoidongnai;Convert Zero Datetime=True";
                con.Open();

                return con;
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public DataTable select(string query, MySqlParameter[] parameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            DataTable dataTable = new DataTable();
            adapter = new MySqlDataAdapter();
            dataTable = null;
            DataSet dataSet = new DataSet();

            try
            {
                cmd.Connection = this.openConnection();
                cmd.CommandText = query;
                cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
                adapter.SelectCommand = cmd;
                adapter.Fill(dataSet);
                dataTable = dataSet.Tables[0];

                return dataTable;
            }
            catch(Exception ex)
            {

            }
            return null;
        }
    }
}