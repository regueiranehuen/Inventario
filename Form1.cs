using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

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
        }
        private void CargarProductos()
        {
            dataGridView1.DataSource = DatabaseHelper.ObtenerProductos();
        }


        private void button1_Click(object sender, EventArgs e)
        {

            Globales.id_producto = CrearIdProducto();

            string nombreProducto = Interaction.InputBox("Nombre producto:", "Datos del producto");
            int cantidad = int.Parse(Interaction.InputBox("Cantidad:", "Datos del producto"));
            int precio = int.Parse(Interaction.InputBox("Precio: ", "Datos del producto"));

            Producto prod = new Producto(Globales.id_producto, nombreProducto, cantidad, precio);
            

            DatabaseHelper.AgregarProducto(prod.Id,prod.Nombre, prod.Cantidad, prod.Precio);
            MessageBox.Show("Producto agregado con éxito");

        }

        int CrearIdProducto()
        {

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Asegúrate de que la fila no sea la fila de entrada nueva (vacía)
                if (!row.IsNewRow)
                {
                    // Obtener el valor de la celda 0 (primera celda) de la fila
                    var valorCeldaSQL = row.Cells[0].Value.ToString();

                    int valorCelda = int.Parse(valorCeldaSQL);

                    Globales.listaIdsProductos.Add(valorCelda);
                }
            }

            for (int i = 0; i < Globales.listaIdsProductos.Count(); i++)
            {
                if (!IndiceEnListaIds(i))
                {
                    return i;
                }
            }

            return Globales.listaIdsProductos.Count();

        }

        private bool IndiceEnListaIds(int indice)
        {
            for (int i = 0; i < Globales.listaIdsProductos.Count(); i++)
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

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
