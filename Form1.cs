using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Oracle.ManagedDataAccess.Client;

namespace Inventario
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Globales.listaProductos.Clear();
            OperacionesListas.AgregarProductosALista();

        }
        private void CargarProductos()
        {
            dataGridView1.DataSource = null;  // Desvincula cualquier origen de datos
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.DataSource = DatabaseHelper.ObtenerProductos();

        }


        private void button1_Click(object sender, EventArgs e) // Botón para crear producto
        {
            int idProducto = CrearIdProducto();
            string nombreProducto = Interaction.InputBox("Nombre producto:", "Datos del producto");
            int cantidad = int.Parse(Interaction.InputBox("Cantidad:", "Datos del producto"));
            decimal precio = decimal.Parse(Interaction.InputBox("Precio: ", "Datos del producto"));

            Producto prod = new Producto(idProducto, nombreProducto, cantidad, precio);
            

            DatabaseHelper.AgregarProducto(prod);
            MessageBox.Show("Producto agregado con éxito");

        }

        // El método devuelve el número más chico sin usarse para asignar como id
        int CrearIdProducto()
        {

            List<int> ids = Globales.listaProductos.Select(p => p.Id).ToList();

            for (int i = 0; i < ids.Count; i++)
            {
                if (!ids.Contains(i))
                {
                    return i; // Para los casos en los que hay un "espacio" entre ids (por ejemplo, id 2 sin usar teniendo id 1 e id 3 ocupados)
                }
            }
            return ids.Count(); // Si el índice recorrió toda la lista y no se encontró espacio entre ids, devolvemos el tamaño de la lista, convirtiéndose el nuevo ID en el más grande de todos


        }




        private void button2_Click(object sender, EventArgs e) // Botón para obtener todos los productos contenidos en la BD
        {
            CargarProductos();
        }

        private void button3_Click(object sender, EventArgs e) // Botón para buscar un producto
        {
            int idBuscado = int.Parse(Interaction.InputBox("Id del producto a buscar:", "Búsqueda del producto"));
            bool busquedaExitosa = BuscarProducto(idBuscado);

            if (!busquedaExitosa)
            {
                MessageBox.Show("El producto buscado no se encuentra en la base de datos");
            }

        }

        private bool BuscarProducto(int idBuscado)
        {

            Producto prod = Globales.listaProductos.Find(p => p.Id == idBuscado);

            if (prod != null)
            {
                dataGridView1.DataSource = null;  // Desvincula cualquier origen de datos
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Add("id_producto", "ID_PRODUCTO");
                dataGridView1.Columns.Add("nombre", "NOMBRE");
                dataGridView1.Columns.Add("cantidad", "CANTIDAD");
                dataGridView1.Columns.Add("precio", "PRECIO");
                dataGridView1.Rows.Add(prod.Id, prod.Nombre, prod.Cantidad, prod.Precio);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EliminarProducto(int idAEliminar)
        {
            Producto prod = Globales.listaProductos.Find(p => p.Id == idAEliminar);

            if (prod != null)
            {

                using (OracleConnection conn = new OracleConnection(Globales.connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM tabla_productos WHERE id_producto = :idAEliminar";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // Agregar el parámetro @idProducto para evitar inyecciones SQL
                        cmd.Parameters.Add(new OracleParameter(":idAEliminar", idAEliminar));
                        // Ejecutar la consulta y obtener el número de filas afectadas
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        Globales.listaProductos.RemoveAll(p=>p.Id == idAEliminar);

                        if (filasAfectadas > 0)
                        {
                            return true;
                        }
                        else
                        {
 
                            return false;
                        }

                    }
                }
            }
            else
            {
                
                return false;
            }
        }

        private void button4_Click(object sender, EventArgs e) // Botón para eliminar producto
        {
            int idAEliminar = int.Parse(Interaction.InputBox("Id del producto a eliminar:", "Eliminación del producto"));
            bool eliminacionConfirmada = EliminarProducto(idAEliminar);
            if (eliminacionConfirmada)
            {
                MessageBox.Show("Producto eliminado con éxito (actualizar tabla para ver el cambio)");
            }
            else
            {
                MessageBox.Show("El producto buscado no se encuentra en la base de datos / No se pudo eliminar el producto");
            }
        }

        // Método de modificación como combinación de buscar, eliminar y agregar
        private void button5_Click(object sender, EventArgs e) // Botón para modificar producto
        {
            int idProdAModificar= int.Parse(Interaction.InputBox("Id del producto a modificar:", "Modificación del producto"));
            
            if (BuscarProducto(idProdAModificar) && EliminarProducto(idProdAModificar)) 
            {
                string nombreProducto = Interaction.InputBox("Nombre producto:", "Modificación del producto");
                int cantidad = int.Parse(Interaction.InputBox("Cantidad:", "Modificación del producto"));
                decimal precio = int.Parse(Interaction.InputBox("Precio: ", "Modificación del producto"));

                Producto prod = new Producto(idProdAModificar, nombreProducto, cantidad, precio);


                DatabaseHelper.AgregarProducto(prod);
                MessageBox.Show("Producto modificado con éxito");
            }
            else
            {
                MessageBox.Show("No se pudo modificar el producto");
            }

        }

    }
}
