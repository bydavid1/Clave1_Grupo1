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
    public partial class Registrar_pedido : Form
    {
        private Connection connection;
        public Registrar_pedido()
        {
            InitializeComponent();

            fillCBProveedor();
            fillCBProductos();
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

        private void fillCBProductos()
        {
            try
            {
                connection = new Connection();

                using (var conn = connection.openConnection())
                {
                    string query = "SELECT id, nombre from productos;";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    MySqlDataAdapter da1 = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    CBProducto.ValueMember = "id";
                    CBProducto.DisplayMember = "nombre";
                    CBProducto.DataSource = dt;

                    connection.closeConnection();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error al obtener la lista de productos");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var producto = CBProducto.SelectedValue;
                var cantidad = (double)ValCantidad.Value;
                connection = new Connection();

                using (var conn = connection.openConnection())
                {
                    string query = $"SELECT * from productos WHERE id={producto};";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    MySqlDataReader res = cmd.ExecuteReader();
                    if (res.Read())
                    {
                        var existencias = Convert.ToInt32(res["existencias"]);
                        if (cantidad > existencias)
                        {
                            MessageBox.Show("No hay cantidad suficiente en stock");
                        } else
                        {
                            var precioFinal = definirPrecioFinal(Convert.ToString(res["talla"]), Convert.ToDouble(res["precio"]));
                            var total = precioFinal * cantidad;
                            GridPedidos.Rows.Add(res["id"], res["nombre"], cantidad, $"${precioFinal}", $"${total}");

                            LblTotal.Text = Convert.ToString(Convert.ToDouble(LblTotal.Text) + total);
                            LblCantTotal.Text = Convert.ToString(Convert.ToDouble(LblCantTotal.Text) + cantidad);
                        }
                    } else
                    {
                        throw new Exception("No se obtuvo una respuesta de la DB");
                    }

                    connection.closeConnection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private double definirPrecioFinal(string talla, double precio)
        {
            if (talla == "XS" || talla == "S")
            {
                return (precio * 0.10) + precio;
            }
            else if(talla == "M" || talla == "L")
            {
                return (precio * 0.15) + precio;
            }
            else if (talla == "XL" || talla == "XXL")
            {
                return (precio * 0.20) + precio;
            }
            else
            {
                return precio;
            }
        }

        private void actualizarExistencias()
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (GridPedidos.Rows.Count > 0)
                {
                    connection = new Connection();

                    using (var con = connection.openConnection())
                    {
                        using (MySqlTransaction trx = con.BeginTransaction())
                        {
                            try
                            {
                                int pedido = registrarPedido(con, trx);
                                registrarItemsPedidos(con, trx, pedido);

                                trx.Commit();
                                MessageBox.Show("Pedido registrado");
                            }
                            catch (Exception)
                            {
                                trx.Rollback();
                                throw;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se han especificado productos");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private int registrarPedido(MySqlConnection con, MySqlTransaction trx)
        {
            int id;
            var proveedor = CBProveedor.SelectedValue;
            string query = $"insert into pedidos (total,cant_total,proveedor_id) " +
                $"values('{LblTotal.Text}','{LblCantTotal.Text}','{proveedor}');";

            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                cmd.Transaction = trx;
                cmd.ExecuteNonQuery();
                id = (int)cmd.LastInsertedId;
            }

            return id;
        }

        private void registrarItemsPedidos(MySqlConnection con, MySqlTransaction trx, int pedido)
        {
            string partialQuery = "";

            for (int rows = 0; rows < GridPedidos.Rows.Count; rows++)
            {
                var product_id = GridPedidos.Rows[rows].Cells[0].Value.ToString();
                var cantidad = GridPedidos.Rows[rows].Cells[2].Value.ToString();
                var precio = GridPedidos.Rows[rows].Cells[3].Value.ToString().Replace('$',' ');
                var total = GridPedidos.Rows[rows].Cells[4].Value.ToString().Replace('$', ' '); ;
                partialQuery += $"({pedido},{product_id},{cantidad},{precio},{total}),";
            }

            string query = $"insert into productos_pedidos " +
                $"(pedido_id, producto_id, cantidad, precio, total) values{partialQuery}";

            string finalQuery = query.Substring(0, query.Length - 1) + ";";
                   

            using (MySqlCommand cmd = new MySqlCommand(finalQuery, con))
            {
                cmd.Transaction = trx;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
