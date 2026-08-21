using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;

namespace inmobiliaria_grupo_9.Models
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration)
        {
        }

        // ALTA
        public int Alta(Inquilino p)
        {
            int res = -1;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO inquilino
                    (Nombre, Apellido, Dni, Telefono, Email)
                    VALUES (@nombre, @apellido, @dni, @telefono, @email);
                    SELECT LAST_INSERT_ID();";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);

                    connection.Open();

                    res = Convert.ToInt32(command.ExecuteScalar());

                    p.IdInquilino = res;

                    connection.Close();
                }
            }

            return res;
        }


        // BAJA
        public int Baja(int id)
        {
            int res = -1;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"DELETE FROM inquilino
                            WHERE ID_Inquilino = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    res = command.ExecuteNonQuery();

                    connection.Close();
                }
            }

            return res;
        }


        // MODIFICACIÓN
        public int Modificacion(Inquilino p)
        {
            int res = -1;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE inquilino
                    SET Nombre = @nombre,
                        Apellido = @apellido,
                        Dni = @dni,
                        Telefono = @telefono,
                        Email = @email
                    WHERE ID_Inquilino = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@id", p.IdInquilino);

                    connection.Open();

                    res = command.ExecuteNonQuery();

                    connection.Close();
                }
            }

            return res;
        }


        // LISTADO CON PAGINADO
        public IList<Inquilino> ObtenerLista(
            int paginaNro = 1,
            int tamPagina = 10)
        {
            IList<Inquilino> res = new List<Inquilino>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT
                        ID_Inquilino AS IdInquilino,
                        Nombre,
                        Apellido,
                        Dni,
                        Telefono,
                        Email
                    FROM inquilino
                    ORDER BY ID_Inquilino
                    LIMIT {tamPagina}
                    OFFSET {(paginaNro - 1) * tamPagina}";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Inquilino p = new Inquilino
                        {
                            IdInquilino =
                                reader.GetInt32(nameof(Inquilino.IdInquilino)),

                            Nombre =
                                reader.GetString(nameof(Inquilino.Nombre)),

                            Apellido =
                                reader.GetString(nameof(Inquilino.Apellido)),

                            Dni =
                                reader.GetString(nameof(Inquilino.Dni)),

                            Telefono =
                                reader.IsDBNull(
                                    reader.GetOrdinal(nameof(Inquilino.Telefono))
                                )
                                ? null
                                : reader.GetString(nameof(Inquilino.Telefono)),

                            Email =
                                reader.GetString(nameof(Inquilino.Email))
                        };

                        res.Add(p);
                    }

                    connection.Close();
                }
            }

            return res;
        }


        // CANTIDAD TOTAL
        public int ObtenerCantidad()
        {
            int res = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT COUNT(ID_Inquilino)
                               FROM ID_Inquilino";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    connection.Open();

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        res = reader.GetInt32(0);
                    }

                    connection.Close();
                }
            }

            return res;
        }


        // OBTENER UN INQUILINO POR ID
        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? p = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT
                        ID_Inquilino  AS IdInquilino,
                        Nombre,
                        Apellido,
                        Dni,
                        Telefono,
                        Email
                    FROM inquilino
                    WHERE ID_Inquilino  = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.Add(
                        "@id",
                        MySqlDbType.Int32
                    ).Value = id;

                    connection.Open();

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        p = new Inquilino
                        {
                            IdInquilino =
                                reader.GetInt32(nameof(Inquilino.IdInquilino)),

                            Nombre =
                                reader.GetString(nameof(Inquilino.Nombre)),

                            Apellido =
                                reader.GetString(nameof(Inquilino.Apellido)),

                            Dni =
                                reader.GetString(nameof(Inquilino.Dni)),

                            Telefono =
                                reader.IsDBNull(
                                    reader.GetOrdinal(nameof(Inquilino.Telefono))
                                )
                                ? null
                                : reader.GetString(nameof(Inquilino.Telefono)),

                            Email =
                                reader.GetString(nameof(Inquilino.Email))
                        };
                    }

                    connection.Close();
                }
            }

            return p;
        }
    }
}