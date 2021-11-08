using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Proyecto_final.Helpers;

namespace Proyecto_final
{
    public partial class Agregar_producto : Form
    {
        private Connection connection;
        public Agregar_producto()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                connection = new Connection();

                string nombre = TxtNombre.Text;
                string codigo = TxtCodigo.Text;
                decimal precio = ValPrecio.Value;
                var talla = CBTalla.SelectedText;

                using (var con = connection.openConnection())
                {
                    string query = $"insert into productos (nombre,codigo,precio,talla) values('{nombre}','{codigo}',{precio},'{talla}');";
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

                            MessageBox.Show("Producto registrado");

                            TxtNombre.Text = "";
                            TxtCodigo.Text = "";
                            ValPrecio.Value = 0;

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
