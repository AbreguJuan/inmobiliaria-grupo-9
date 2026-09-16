using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace inmobiliaria_grupo_9.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Usuario e)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Usuario 
                    (Nombre, Apellido, Avatar, Email, Clave, Rol) 
                    VALUES (@nombre, @apellido, @avatar, @email, @clave, @rol);
                    SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", e.Nombre);
                    command.Parameters.AddWithValue("@apellido", e.Apellido);
                    
                    if (string.IsNullOrEmpty(e.Avatar))
                        command.Parameters.AddWithValue("@avatar", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@avatar", e.Avatar);
                        
                    command.Parameters.AddWithValue("@email", e.Email);
                    command.Parameters.AddWithValue("@clave", e.Clave);
                    command.Parameters.AddWithValue("@rol", e.Rol);
                    
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    e.IdUsuario = res;
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM Usuario WHERE ID_Usuario = @id";
                using (var command = new MySqlCommand(sql, connection))
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

        public int Modificacion(Usuario e)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Usuario 
                    SET Nombre=@nombre, Apellido=@apellido, Avatar=@avatar, Email=@email, Clave=@clave, Rol=@rol
                    WHERE ID_Usuario = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", e.Nombre);
                    command.Parameters.AddWithValue("@apellido", e.Apellido);
                    command.Parameters.AddWithValue("@avatar", string.IsNullOrEmpty(e.Avatar) ? DBNull.Value : e.Avatar);
                    command.Parameters.AddWithValue("@email", e.Email);
                    command.Parameters.AddWithValue("@clave", e.Clave);
                    command.Parameters.AddWithValue("@rol", e.Rol);
                    command.Parameters.AddWithValue("@id", e.IdUsuario);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Usuario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Usuario> res = new List<Usuario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT ID_Usuario AS IdUsuario, Nombre, Apellido, Avatar, Email, Clave, Rol
                    FROM Usuario
                    ORDER BY ID_Usuario
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Usuario e = new Usuario
                        {
                            IdUsuario = reader.GetInt32("IdUsuario"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Avatar = reader["Avatar"] is DBNull ? null : reader.GetString("Avatar"),
                            Email = reader.GetString("Email"),
                            Clave = reader.GetString("Clave"),
                            Rol = reader.GetInt32("Rol"),
                        };
                        res.Add(e);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int ObtenerCantidad()
        {
            int res = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(ID_Usuario) FROM Usuario";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? e = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT ID_Usuario AS IdUsuario, Nombre, Apellido, Avatar, Email, Clave, Rol 
                    FROM Usuario
                    WHERE ID_Usuario=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        e = new Usuario
                        {
                            IdUsuario = reader.GetInt32("IdUsuario"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Avatar = reader["Avatar"] is DBNull ? null : reader.GetString("Avatar"),
                            Email = reader.GetString("Email"),
                            Clave = reader.GetString("Clave"),
                            Rol = reader.GetInt32("Rol"),
                        };
                    }
                    connection.Close();
                }
            }
            return e;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? e = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT ID_Usuario AS IdUsuario, Nombre, Apellido, Avatar, Email, Clave, Rol 
                    FROM Usuario
                    WHERE Email=@email";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@email", email);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        e = new Usuario
                        {
                            IdUsuario = reader.GetInt32("IdUsuario"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Avatar = reader["Avatar"] is DBNull ? null : reader.GetString("Avatar"),
                            Email = reader.GetString("Email"),
                            Clave = reader.GetString("Clave"),
                            Rol = reader.GetInt32("Rol"),
                        };
                    }
                    connection.Close();
                }
            }
            return e;
        }
    }
}