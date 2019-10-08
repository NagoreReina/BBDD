using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HotelProgram
{
    class Program
    {
        static SqlConnection connection = new SqlConnection("Data Source=DESKTOP-C1JLP92\\SQLEXPRESS;Initial Catalog=Hotel;Integrated Security=True");
        static void Main(string[] args)
        {   
            Menu();   
        }
        public static void Menu()
        {
            bool closeProgram = false;
            do
            {
                Console.WriteLine("---***** HOTEL *****---");
                Console.WriteLine("Bienvenid@, por favor, elige una opción");
                Console.WriteLine("___________________________________");
                Console.WriteLine("1. Registrarse");
                Console.WriteLine("2. Editar datos");
                Console.WriteLine("3. Check In");
                Console.WriteLine("4. Check Out");
                Console.WriteLine("5. Salir");
                Console.WriteLine("___________________________________");
                int option = 0;
                if (int.TryParse(Console.ReadLine(), out option))
                {
                    switch (option)
                    {
                        case 1:
                            ModificarBase(RegistrarCliente());
                            break;
                        case 2:
                            Console.WriteLine("Introduce tu DNI");
                            string dni = Console.ReadLine();
                            if (dni.Length < 11 && dni != "")
                            {
                                string query = $"SELECT Dni FROM Clientes WHERE dni = '{dni}'";
                                if (ConsultarBase(query))
                                {
                                    ModificarBase(EditarCliente(dni));
                                }
                                else
                                {
                                    Console.WriteLine("El dni introducido no es correcto");
                                }
                            }
                            else
                            {
                                Console.WriteLine("El dni introducido no es correcto");
                            }

                            break;
                        case 3:
                            Console.WriteLine("Introduce tu DNI");
                            dni = Console.ReadLine();
                            if (dni.Length < 11 && dni != "")
                            {
                                string query = $"SELECT Dni FROM Clientes WHERE dni = '{dni}'";
                                if (ConsultarBase(query))
                                {
                                    ModificarBase(CheckIn(dni));
                                }
                                else
                                {
                                    Console.WriteLine("Este DNI no esta registrado, por favor, registrese antes de hacer el CheckIn");
                                }
                            }
                            else
                            {
                                Console.WriteLine("El dni introducido no es correcto");
                            }

                            break;
                        case 4:
                            Console.WriteLine("Introduce tu DNI");
                            dni = Console.ReadLine();
                            if (dni.Length < 11 && dni != "")
                            {
                                string query = $"SELECT Dni FROM Clientes WHERE dni = '{dni}'";
                                if (ConsultarBase(query))
                                {
                                    int id = IDClienteFromDni(dni);
                                    query = $"SELECT * FROM Reservas WHERE idCliente = '{id}'";
                                    if (ConsultarBase(query))
                                    {
                                        ModificarBase(CheckOut(dni));
                                    }
                                    else
                                    {
                                        Console.WriteLine("Este Cliente no tiene ninguna habitación asignada");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Este DNI no esta registrado, por favor, registrese antes de hacer el CheckIn");
                                }
                            }
                            else
                            {
                                Console.WriteLine("El dni introducido no es correcto");
                            }

                            break;
                        case 5:
                            Console.WriteLine("Gracias por tu visita, esperamos verte pronto");
                            closeProgram = true;
                            break;
                        default:
                            Console.WriteLine("La opción que has introducido no es valida, prueba otra vez");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Valor incorrecto, por favor, introduce un número");
                }
            } while (!closeProgram);
            
        }
        public static string RegistrarCliente()
        {
            bool correctClient = false;
            do
            {
                Console.WriteLine("Creando un nuevo Cliente, por favor, introduce tu nombre");
                string name = Console.ReadLine().ToLower();
                if (name.Length < 16 && name != "")
                {
                    Console.WriteLine("ahora introduce tu apellido");
                    string lastName = Console.ReadLine().ToLower();
                    if (lastName.Length < 31 && lastName != "")
                    {
                        Console.WriteLine("ahora introduce tu DNI");
                        string dni = Console.ReadLine().ToLower();
                        if (dni.Length < 11 && dni != "")
                        {
                            string query = $"INSERT INTO Clientes(Nombre, Apellido, DNI) VALUES ('{name}','{lastName}','{dni}')";
                            return query;
                        }
                        else
                        {
                            Console.WriteLine("El DNI introducido no es valido");
                        }
                    }
                    else
                    {
                        Console.WriteLine("El Apellido introducido no es valido");
                    }
                }
                else
                {
                    Console.WriteLine("El nombre introducido no es valido");
                }
            } while (!correctClient);
            
            return null;

        }
        public static string EditarCliente(string dni)
        {
            Console.WriteLine("Editando datos de Cliente, por favor, introduce tu NUEVO nombre");
            string name = Console.ReadLine().ToLower();
            if (name.Length < 16 && name != "")
            {
                Console.WriteLine("ahora introduce tu NUEVO apellido");
                string lastName = Console.ReadLine().ToLower();
                if (lastName.Length < 31 && lastName != "")
                {   
                    string query = $"UPDATE Clientes SET Nombre = '{name}', Apellido = '{lastName}' WHERE Dni = '{dni.ToLower()}'";
                    return query;                    
                }
                else
                {
                    Console.WriteLine("El Apellido introducido no es valido");
                }
            }
            else
            {
                Console.WriteLine("El nombre introducido no es valido");
            }
            return null;
        }
        public static string CheckIn(string dni)
        {
            Console.WriteLine("___________________________________");
            Console.WriteLine("Estas son las habitaciones disponibles: ");
            //Cadena de consexión con la base de dato
            string query = "SELECT ID FROM Habitaciones WHERE Estado = 'Libre'";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            for (int i = 0; reader.Read(); i++)
            {
                Console.Write($"{reader[0].ToString()}, ");
            }
            connection.Close();
            Console.WriteLine("\n");
            Console.WriteLine("¿Que habitación quieres?");
            int option = 0;
            if (int.TryParse(Console.ReadLine(), out option))
            {
                Console.WriteLine("Habitación reservada, muchas gracias");
                //Añadir en la tabla de reservas
                int id = IDClienteFromDni(dni);
                query = $"INSERT INTO Reservas (IdCliente, IdHabitacion, fechaCheckIn) VALUES ({id},{option},'{DateTime.Now.ToString("dd/MM/yyyy")}')";
                ModificarBase(query);
                //Modificar el estado de la habitación para marcarlo como ocupado
                query = $"UPDATE Habitaciones SET Estado = 'Ocupado' WHERE ID = {option}";
            }
            else
            {
                Console.WriteLine("No has introducido una habitación valida");
            }
            return query;
        }
        public static string CheckOut(string dni)
        {
            //Añadir en la tabla de reservas
            int id = IDClienteFromDni(dni);

            string query = $"SELECT IDhabitacion From Reservas WHERE IdCliente = {id} AND FechaCheckOut is Null";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<int> habitaciones = new List<int>();
            while (reader.Read())
            {
                habitaciones.Add (Convert.ToInt32(reader[0].ToString()));
            }
            connection.Close();
            int seleccion = 0;
            if (habitaciones.Count > 1)
            {
                Console.WriteLine("¿Que habitación quieres dejar? Tienes estas habitaciones: ");
                for (int i = 0; i < habitaciones.Count; i++)
                {
                    Console.Write($"{habitaciones[i]}, ");
                }
                Console.WriteLine("\n");
                if (!int.TryParse(Console.ReadLine(), out seleccion))
                {
                    Console.WriteLine(" No has seleccionado una opción valida");
                }
            }
            else
            {
                seleccion = habitaciones[0];
            }

            query = $"UPDATE Reservas SET fechaCheckOut ='{DateTime.Now.ToString("dd/MM/yyyy")}' WHERE IdHabitacion = {seleccion}";
            ModificarBase(query);

            //Modificar el estado de la habitación para marcarlo como ocupado
            query = $"UPDATE Habitaciones SET Estado = 'Libre' WHERE ID = {seleccion}";
            Console.WriteLine("Muchas gracias por su visita, el check out se realizó correctamente");
            return query;
        }
        public static bool ConsultarBase(string query)
        {
            //Cadena de consexión con la base de dato
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                connection.Close();
                return true;
            }
            connection.Close();
            return false;
        }
        public static void ModificarBase(string query)
        {
            if (query != null)
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public static int IDClienteFromDni(string dni)
        {
            string query = $"SELECT ID FROM Clientes WHERE DNI = '{dni}'";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            int num = 0;
            if (reader.Read())
            {
                num = Convert.ToInt32(reader[0].ToString());
            }
            connection.Close();
            return num;
        }
    }
}
