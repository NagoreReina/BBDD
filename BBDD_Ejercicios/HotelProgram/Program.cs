using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HotelProgram
{
    class Program
    {
        //Conexión principal con la base de datos
        static SqlConnection connection = new SqlConnection("Data Source=DESKTOP-C1JLP92\\SQLEXPRESS;Initial Catalog=Hotel;Integrated Security=True");
        static void Main(string[] args)
        {
            Menu();
        }
        //Función principal
        public static void Menu()
        {
            bool closeProgram = false;
            do
            {
                //Texto del menu, se muestra por pantalla
                Console.WriteLine("---***** HOTEL *****---");
                Console.WriteLine("Bienvenid@, por favor, elige una opción");
                Console.WriteLine("___________________________________");
                Console.WriteLine("1. Registrarse");
                Console.WriteLine("2. Editar datos");
                Console.WriteLine("3. Check In");
                Console.WriteLine("4. Check Out");
                Console.WriteLine("5. Salir");
                Console.WriteLine("6. Consultas");
                Console.WriteLine("7. Reserva con antelación");
                Console.WriteLine("___________________________________");
                int option = 0;
                if (int.TryParse(Console.ReadLine(), out option)) //comprobar que el numero introducido es un numero
                {
                    switch (option)
                    {
                        case 1: //registro
                            string datosRegistro = RegistrarCliente();
                            if (datosRegistro != null) //si el dni introducido ya estaba registrado, devuelve null
                            {
                                ModificarBase(datosRegistro);
                            }
                            
                            break;
                        case 2: //editar datos
                            Console.WriteLine("Introduce tu DNI");
                            string dni = Console.ReadLine();
                            if (dni.Length < 11 && dni != "") //comprobar que sea un dni valido
                            {
                                string query = $"SELECT Dni FROM Clientes WHERE dni = '{dni}'";
                                if (ConsultarBase(query)) //mirar que sea un cliente registrado
                                {
                                    ModificarBase(EditarCliente(dni)); //editar los datos de cliente
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
                        case 3: //checkIn
                            Console.WriteLine("Introduce tu DNI"); 
                            dni = Console.ReadLine();
                            if (dni.Length < 11 && dni != "") //comprobar que sea un dni valido
                            {
                                string query = $"SELECT Dni FROM Clientes WHERE dni = '{dni}'";
                                if (ConsultarBase(query)) //mirar que sea un cliente registrado
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
                        case 4: //checkOut
                            Console.WriteLine("Introduce tu DNI"); 
                            dni = Console.ReadLine();
                            if (dni.Length < 11 && dni != "") //comprobar que sea un dni valido
                            {
                                string query = $"SELECT Dni FROM Clientes WHERE dni = '{dni}'";
                                if (ConsultarBase(query)) //mirar que sea un cliente registrado
                                {
                                    int id = IDClienteFromDni(dni); //transformar el Dni en el id del cliente
                                    query = $"SELECT * FROM Reservas WHERE idCliente = '{id}'";
                                    if (ConsultarBase(query)) //mirar que este cliente tenga una habitación
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
                        case 5: //salir
                            Console.WriteLine("Gracias por tu visita, esperamos verte pronto");
                            closeProgram = true;
                            break;
                        case 6:
                            SubMenu(); //Menu de consultas
                            break;
                        case 7: //Reserva con antelación

                            Console.WriteLine("Introduce tu DNI");
                            dni = Console.ReadLine();
                            if (dni.Length < 11 && dni != "") //comprobar que sea un dni valido
                            {
                                string query = $"SELECT Dni FROM Clientes WHERE dni = '{dni}'";
                                if (ConsultarBase(query)) //mirar que sea un cliente registrado
                                {
                                    ReservaConAntelacion(dni);
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
                if (name.Length < 16 && name != "") //comprobar que sea un nombre valido
                {
                    Console.WriteLine("ahora introduce tu apellido");
                    string lastName = Console.ReadLine().ToLower();
                    if (lastName.Length < 31 && lastName != "") //comprobar que sea un apellido valido
                    {
                        Console.WriteLine("ahora introduce tu DNI");
                        string dni = Console.ReadLine().ToLower();
                        if (dni.Length < 11 && dni != "") //comprobar que sea un dni valido
                        {
                            string query = $"SELECT * FROM Clientes WHERE DNI = '{dni}'";
                            if (!ConsultarBase(query)) //mirar si el dni ya estaba registrado de antes
                            {
                                query = $"INSERT INTO Clientes(Nombre, Apellido, DNI) VALUES ('{name}','{lastName}','{dni}')";
                                correctClient = true;
                                Console.WriteLine("Registro realizado correctamente");
                                return query;
                            }
                            else
                            {
                                Console.WriteLine("El DNI introducido ya esta registrado, intentalo con otro DNI o haz el check in");
                                correctClient = true;
                            }
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
            if (name.Length < 16 && name != "") //comprobar que sea un nombre valido
            {
                Console.WriteLine("ahora introduce tu NUEVO apellido");
                string lastName = Console.ReadLine().ToLower();
                if (lastName.Length < 31 && lastName != "") //comprobar que sea un apellido valido
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
            //TE DA HABITACIONES QUE ESTAN RESERVADAS
            Console.WriteLine("___________________________________");
            Console.WriteLine("Estas son las habitaciones disponibles: ");
            //Cadena de consexión con la base de dato
            string query = "SELECT ID FROM Habitaciones WHERE Estado = 'Libre'";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<int> habitaciones = new List<int>();
            for (int i = 0; reader.Read(); i++)
            {
                habitaciones.Add(Convert.ToInt32(reader[0].ToString()));
                Console.Write($"{reader[0].ToString()}, ");
            }
            connection.Close();
            Console.WriteLine("\n");
            Console.WriteLine("¿Que habitación quieres?");
            int option = 0;
            if (int.TryParse(Console.ReadLine(), out option))
            {
                bool habitacionSeleccionable = false;
                for (int i = 0; i < habitaciones.Count; i++)
                {
                    if (option == habitaciones[i])
                    {
                        habitacionSeleccionable = true;
                    }
                }
                if (habitacionSeleccionable)
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
        public static void SubMenu()
        {
            Console.WriteLine("--------------------");
            Console.WriteLine("¿Qué quieres consultar?");
            Console.WriteLine("___________________________________");
            Console.WriteLine("1. Ver listado de todas las habitaciones");
            Console.WriteLine("2. Ver listado de ocupadas");
            Console.WriteLine("3. Ver listado de habitaciones vacías");
            Console.WriteLine("___________________________________");
            int option = 0;
            if (int.TryParse(Console.ReadLine(), out option))
            {
                switch (option)
                {
                    case 1:
                        Console.WriteLine("___________________________________");
                        Console.WriteLine("Listado de todas las habitaciones:");
                        string query = "SELECT ID, Estado FROM Habitaciones WHERE Estado = 'Libre'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        for (int i = 0; reader.Read(); i++)
                        {
                            Console.WriteLine( $"nº {reader[0].ToString()} - Estado: {reader["Estado"].ToString()}");
                        }
                        connection.Close();
                        List<string> text = new List<string>();
                        text = ClientesEnHabitaciones();
                        for (int i = 0; i < text.Count; i++)
                        {
                            Console.WriteLine(text[i]);
                        }
                        Console.WriteLine("___________________________________");
                        break;
                    case 2:
                        Console.WriteLine("___________________________________");
                        Console.WriteLine("Listado de todas las habitaciones Ocupadas:");
                        text = ClientesEnHabitaciones();
                        for (int i = 0; i < text.Count; i++)
                        {
                            Console.WriteLine(text[i]);
                        }
                        Console.WriteLine("___________________________________");
                        break;
                    case 3:
                        Console.WriteLine("___________________________________");
                        Console.WriteLine("Listado de todas las habitaciones Libres:");
                        query = "SELECT ID, Estado FROM Habitaciones WHERE Estado = 'Libre'";
                        command = new SqlCommand(query, connection);
                        connection.Open();
                        reader = command.ExecuteReader();
                        for (int i = 0; reader.Read(); i++)
                        {
                            Console.WriteLine($"nº {reader[0].ToString()} - Estado: {reader["Estado"].ToString()}");
                        }
                        connection.Close();
                        Console.WriteLine("___________________________________");
                        break;
                    default:
                        Console.WriteLine("La opción que has introducido no es valida, prueba otra vez");
                        break;
                }
            }
        }
        public static List<string> ClientesEnHabitaciones()
        {
            string query = $"SELECT IDCliente, IDhabitacion FROM Reservas WHERE FechaCheckOut is Null ";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<String> Text = new List<string>();
            while (reader.Read())
            {
                Text.Add($"{reader[0].ToString()}-nº {reader[1].ToString()}");
            }
            connection.Close();
                
            for (int i = 0; i < Text.Count; i++)
            {
                string[] temp = Text[i].Split('-');
                int n = Convert.ToInt32(temp[0]); //ID CLIENTE
                query = $"SELECT Nombre, Apellido FROM Clientes WHERE Id = {n}";
                command = new SqlCommand(query, connection);
                connection.Open();
                reader = command.ExecuteReader();
                string nombreCliente = "";
                
                if (reader.Read())
                {
                    nombreCliente = $"{reader[0].ToString()} {reader[1].ToString()}";
                    Text[i] = $"{temp[1]} - Estado: Ocupada - Cliente: {nombreCliente}"; 

                }
                connection.Close();
            }   
            return Text;
        }
        public static void ReservaConAntelacion(string dni)
        {

        }
    }
}
