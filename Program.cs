using System;
using System.Data;
using System.IO;
using Oracle.ManagedDataAccess.Client;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Inventario
{
    public static class Globales
    {
        // Cadena de conexión adaptada a Oracle
        public static string connectionString = "User Id=Nehuen;Password=NomelaOracle33;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=FREEXDB)))";


        public static int id_producto = 0;

        public static List<int> listaIdsProductos = new List<int>();

    }

    public static class DatabaseHelper
    {
        // Método para agregar un producto
        public static void AgregarProducto(int id_producto, string nombre, int cantidad, decimal precio)
        {
            using (OracleConnection conn = new OracleConnection(Globales.connectionString))
            {
                conn.Open();
                string query = "INSERT INTO TABLA_PRODUCTOS (ID_PRODUCTO, NOMBRE, CANTIDAD, PRECIO) VALUES (:id_producto, :nombre, :cantidad, :precio)";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(":id_producto", OracleDbType.Int32).Value = id_producto;
                    cmd.Parameters.Add(":nombre", OracleDbType.Varchar2).Value = nombre;
                    cmd.Parameters.Add(":cantidad", OracleDbType.Int32).Value = cantidad;
                    cmd.Parameters.Add(":precio", OracleDbType.Decimal).Value = precio;
                    cmd.ExecuteNonQuery();
                }
            }
            ObtenerProductos();
        }

        // Método para obtener productos
        public static DataTable ObtenerProductos()
        {
            using (OracleConnection conn = new OracleConnection(Globales.connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM TABLA_PRODUCTOS";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    // Leer un archivo SQL y ejecutar una consulta (opcional)
                    string filePath = "C:\\Users\\nehue\\Escritorio\\Programacion\\ProyectosDesktopApps\\Inventario\\Statements\\ContarFilas.sql"; // Ruta al archivo SQL
                    if (File.Exists(filePath))
                    {
                        string queryFromFile = File.ReadAllText(filePath);
                        using (OracleCommand cmd2 = new OracleCommand(queryFromFile, conn))
                        {
                            //Globales.id_producto = Convert.ToInt32(cmd2.ExecuteScalar());
                            int cantProductos = Convert.ToInt32(cmd2.ExecuteScalar());
                            MessageBox.Show($"Cantidad de productos: {cantProductos}");
                        }

                    }
                    else
                    {
                        MessageBox.Show("PINGO");
                    }


                    return table;
                }
            }
        }
    }

    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }

        // Constructor
        public Producto(int id, string nombre, decimal precio, int cantidad)
        {
            Id = id;
            Nombre = nombre;
            Precio = precio;
            Cantidad = cantidad;
        }

        // Método para imprimir el objeto como texto
        public override string ToString()
        {
            return $"Id: {Id}, Nombre: {Nombre}, Precio: {Precio}, Cantidad: {Cantidad}";
        }
    }
}
