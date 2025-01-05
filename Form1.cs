using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Producto prod = new Producto(Globales.id_producto, "producto_prueba", 30, 2);
            

            DatabaseHelper.AgregarProducto(Globales.id_producto,prod.Nombre, prod.Cantidad, prod.Precio);
            MessageBox.Show("Producto agregado con éxito");
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
