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
    public partial class Agregar_proveedor : Form
    {
        private Connection connection;
        public Agregar_proveedor()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                connection = new Connection();

                string nombre = TxtNombre.Text;
                string contacto = TxtContacto.Text;

                using (var con = connection.openConnection())
                {
                    string query = $"insert into proveedores (nombre,contacto) values('{nombre}','{contacto}');";
                    using (MySqlTransaction trx = con.BeginTransaction())
                    {
                        try
                        {
                            using (MySqlCommand cmd = new MySqlCommand(query, con))
                            {
                                cmd.Transaction = trx;
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();

                            MessageBox.Show("Proveedor registrado");

                            TxtNombre.Text = "";
                            TxtContacto.Text = "";
                        }
                        catch (Exception)
                        {
                            trx.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
