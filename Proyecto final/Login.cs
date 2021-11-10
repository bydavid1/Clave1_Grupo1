using MySql.Data.MySqlClient;
using Proyecto_final.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_final
{
    public partial class Login : Form
    {
        private Connection connection;
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = TxtUsuario.Text;
                string password = TxtPass.Text;

                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Ingrese usuario y contraseña");
                }
                else
                {
                    connection = new Connection();
                    using (var conn = connection.openConnection())
                    {
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = "select * from usuarios where (username=@user and password=@pass)";
                        cmd.Parameters.AddWithValue("@user", usuario);
                        cmd.Parameters.AddWithValue("@pass", password);
                        cmd.CommandType = CommandType.Text;
                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            var dashboard = new Form1();
                            dashboard.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Usuario incorrecto");
                        }

                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Ocurrió un problema, verifica los datos");
            }
        }
    }
}
