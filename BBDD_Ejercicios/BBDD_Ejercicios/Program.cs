using System;
using System.Data.SqlClient;

namespace BBDD_Ejercicios
{
    class Program
    {
        static void Main(string[] args)
        {
            //Cadena de consexión con la base de datos
            SqlConnection connection = new SqlConnection("Data Source=DESKTOP-C1JLP92\\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True");

            //Query para introducir los comandos
            //---string query = "SELECT * FROM Employees";

            //string country = "ESP";
            //string query = $"UPDATE Employees SET Country = '{country}' WHERE Country = 'AUS'";
            bool completedProgram = false;
            do
            {
                string query = "";
                Console.WriteLine("Introduce tu apellido");
                string lastName = Console.ReadLine().ToLower();
                query = $"SELECT * FROM Employees WHERE LastName = '{lastName}'";

                //de esta manera usamos la conexion y el comando para trabajar con la base de datos
                SqlCommand command = new SqlCommand(query, connection);

                //abrir la conexión
                connection.Open();

                //Lee los registros - guarda todo lo que lea el comando -
                SqlDataReader reader = command.ExecuteReader();

                //Ejecuta el comando 
                //Console.WriteLine(command.ExecuteNonQuery() + " Han cambiado");

                //---Mientras haya registro que leer, este codigo se estará ejecutando
                //while (reader.Read())
                //{
                //    for (int i = 0; i < 18; i++)
                //    {
                //        Console.Write($"| { reader[i].ToString()} |");
                //    }
                //    Console.WriteLine("\n");
                //}
                if (!reader.Read())
                {
                    Console.WriteLine("Apellido Incorrecto, prueba otra vez");
                    connection.Close();
                }
                else
                {
                    connection.Close();
                    Console.WriteLine("Introduce tu nuevo pais");
                    string country = Console.ReadLine().ToUpper();
                    query = $"UPDATE Employees SET Country = '{country}' WHERE LastName = '{lastName}'";
                    command = new SqlCommand(query, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    completedProgram = true;
                    connection.Close();
                    Console.WriteLine("Hemos cambiado tu pais correctamente");
                }
            } while (!completedProgram);
            
            
            
        }
    }
}
