﻿using System;
using System.Data;
using System.IO;
using Oracle.ManagedDataAccess.Client;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Inventario
{
    public static class Globales
    {
        // Cadena de conexión adaptada a Oracle
        public static string connectionString = "User Id=Nehuen;Password=NomelaOracle33;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=FREEXDB)))";


        public static int id_producto = 0;

        public static List<Producto> listaProductos = new List<Producto>();

    }

    public static class OperacionesListas
    {
        public static void AgregarProductosALista()
        {
            
            /*foreach (DataGridViewRow row in dgv.Rows)
            {
                // Asegúrate de que la fila no sea la fila de entrada nueva (vacía)
                if (!row.IsNewRow)
                {
                    // Obtener el valor de la celda 0 (primera celda) de la fila
                    int id = int.Parse(row.Cells[0].Value.ToString());

                    string nombre = (row.Cells[1].Value.ToString());
                    int cant = int.Parse(row.Cells[2].Value.ToString());
                    int precio = int.Parse(row.Cells[3].Value.ToString());

                    Producto prod = new Producto(id, nombre, cant, precio);

                    //if (!IndiceEnListaIds(valorCelda))

                    if (Globales.listaProductos.Find(p => p.Id == id)!=null)
                        Globales.listaProductos.Add(prod);
                }
            }*/

            // Cadena de conexión (ajusta según tu servidor y base de datos)

            // Consulta SQL
            string query = "SELECT id_producto, nombre, cantidad, precio FROM TABLA_PRODUCTOS";

            try
            {
                // Abrir conexión y leer datos
                using (OracleConnection conn = new OracleConnection(Globales.connectionString))
                {
                    conn.Open();

                    using (OracleCommand command = new OracleCommand(query, conn))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                int id = int.Parse(reader.GetString(0));
                                string nombre = reader.GetString(1);
                                int cant = int.Parse(reader.GetString(2));
                                int precio = int.Parse(reader.GetString(3));

                                Producto prod = new Producto(id, nombre, cant, precio);
                                //Producto producto = new Producto(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetDecimal(2));

                                Globales.listaProductos.Add(prod);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            //return listaProductos;
        }

    }

    public static class DatabaseHelper
    {
        // Método para agregar un producto
        public static void AgregarProducto(Producto prod)
        {
            using (OracleConnection conn = new OracleConnection(Globales.connectionString))
            {
                conn.Open();
                if (Globales.listaProductos.Find(p=>p.Id == prod.Id) == null)
                {
                    string query = "INSERT INTO TABLA_PRODUCTOS (ID_PRODUCTO, NOMBRE, CANTIDAD, PRECIO) VALUES (:id_producto, :nombre, :cantidad, :precio)";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":id_producto", OracleDbType.Int32).Value = prod.Id;
                        cmd.Parameters.Add(":nombre", OracleDbType.Varchar2).Value = prod.Nombre;
                        cmd.Parameters.Add(":cantidad", OracleDbType.Int32).Value = prod.Cantidad;
                        cmd.Parameters.Add(":precio", OracleDbType.Decimal).Value = prod.Cantidad;
                        cmd.ExecuteNonQuery();
                        Globales.listaProductos.Add(prod);

                    }
                }
                else
                {
                    MessageBox.Show("Error: El producto ya se encuentra en la base de datos");
                }
                
            }
            //ObtenerProductos();
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

                            //MessageBox.Show($"Cantidad de productos: {cantProductos}");
                        }

                    }
                    else
                    {
                        //MessageBox.Show("PINGO");
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
        public Producto(int id, string nombre, int cantidad, decimal precio)
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
