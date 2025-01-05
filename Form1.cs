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
            //Form form = sender as Form;
            //this.CargarProductos();
            OperacionesListas.AgregarProductosALista();

        }
        private void CargarProductos()
        {
            dataGridView1.DataSource = null;  // Desvincula cualquier origen de datos
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            /*dataGridView1.Columns.Add("id_producto", "ID_PRODUCTO");
            dataGridView1.Columns.Add("nombre", "NOMBRE");
            dataGridView1.Columns.Add("cantidad", "CANTIDAD");
            dataGridView1.Columns.Add("precio", "PRECIO");*/
            //dataGridView1.Rows.Add(prod.Id, prod.Nombre, prod.Cantidad, prod.Precio);
            dataGridView1.DataSource = DatabaseHelper.ObtenerProductos();
            //OperacionesListas.AgregarProductosALista(dataGridView1);
        }


        private void button1_Click(object sender, EventArgs e)
        {

            Globales.id_producto = CrearIdProducto();

            string nombreProducto = Interaction.InputBox("Nombre producto:", "Datos del producto");
            int cantidad = int.Parse(Interaction.InputBox("Cantidad:", "Datos del producto"));
            decimal precio = decimal.Parse(Interaction.InputBox("Precio: ", "Datos del producto"));

            Producto prod = new Producto(Globales.id_producto, nombreProducto, cantidad, precio);
            

            DatabaseHelper.AgregarProducto(prod);
            MessageBox.Show("Producto agregado con éxito");

        }

        int CrearIdProducto()
        {

            List<int> ids = Globales.listaProductos.Select(p => p.Id).ToList();

            for (int i = 0; i < ids.Count; i++)
            {
                if (!IndiceEnLista(i, ids))
                    return i;
            }
            return ids.Count();


        }

        private bool IndiceEnLista(int indice, List<int> lista)
        {
            for (int i = 0; i < lista.Count(); i++)
            {
                if (i == indice)
                {
                    return true;
                }
            }
            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CargarProductos();
        }

        private void button3_Click(object sender, EventArgs e)
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
                        int rowsAffected = cmd.ExecuteNonQuery();

                        Globales.listaProductos.RemoveAll(p=>p.Id == idAEliminar);

                        // Verificar cuántas filas fueron eliminadas
                        if (rowsAffected > 0)
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

        private void button4_Click(object sender, EventArgs e)
        {
            int idAEliminar = int.Parse(Interaction.InputBox("Id del producto a eliminar:", "Eliminación del producto"));
            bool eliminacionConfirmada = EliminarProducto(idAEliminar);
            if (eliminacionConfirmada)
            {
                MessageBox.Show("Producto eliminado con éxito (tras obtener productos ya no estará)");
            }
            else
            {
                MessageBox.Show("El producto buscado no se encuentra en la base de datos / No se pudo eliminar el producto");
            }
        }

        private void button5_Click(object sender, EventArgs e)
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
