using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_final.Helpers
{
    public class Connection
    {
        private static string strConnection = "SERVER=localhost; DATABASE=clave1_grupo1db;" +
            "UID=root; PASSWORD=ysklpepe123";
        private MySqlConnection connection;

        public MySqlConnection openConnection()
        {
            connection = new MySqlConnection(strConnection);
            connection.Open();
            return connection;
        }

        public void closeConnection()
        {
            connection.Dispose();
        }
    }
}
