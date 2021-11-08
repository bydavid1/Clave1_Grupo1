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
    public partial class Form1 : Form
    {
        private Connection connection;
        public Form1()
        {
            InitializeComponent();
            fillCBProveedor();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var proveedores = new Proveedores();
            proveedores.Show();
        }

        private void agregarProductoToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var newProduct = new Agregar_producto();
            newProduct.ShowDialog();
        }

        private void agregarProveedorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var newProvider = new Agregar_proveedor();
            newProvider.ShowDialog();
        }

        private void registrarPedidoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var newOrder = new Registrar_pedido();
            newOrder.ShowDialog();
        }

        private void fillCBProveedor()
        {
            try
            {
                connection = new Connection();

                using (var conn = connection.openConnection())
                {
                    string query = "SELECT id, nombre from proveedores";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    MySqlDataAdapter da1 = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    CBProveedor.ValueMember = "id";
                    CBProveedor.DisplayMember = "nombre";
                    CBProveedor.DataSource = dt;

                    connection.closeConnection();
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Error al obtener la lista de proveedores");
            }
        }

        private void CBProveedores_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                var proveedor = CBProveedor.SelectedValue;
                connection = new Connection();

                using (var conn = connection.openConnection())
                {
                    string query = $"SELECT * FROM productos_pedidos JOIN productos ON productos.id = productos_pedidos.id" +
                        $" WHERE productos_pedidos.proveedor_id = {proveedor};";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    //MySqlDataAdapter res = cmd.ExecuteReader();
                    


                    connection.closeConnection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
