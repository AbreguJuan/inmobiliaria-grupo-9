using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace inmobiliaria_grupo_9.Models
{
    public class RepositorioTipoDeInmueble : RepositorioBase, IRepositorioTipoDeInmueble
    {
        public RepositorioTipoDeInmueble(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(TipoDeInmueble t)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO tipo_inmueble (Nombre, Habilitado)
                    VALUES (@nombre, @habilitado);
                    SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", t.Nombre);
                    command.Parameters.AddWithValue("@habilitado", t.Habilitado);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    t.IdTipoInmueble = res;
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
                string sql = "DELETE FROM tipo_inmueble WHERE ID_TipoInmueble = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(TipoDeInmueble t)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE tipo_inmueble
                    SET Nombre = @nombre, Habilitado = @habilitado
                    WHERE ID_TipoInmueble = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", t.Nombre);
                    command.Parameters.AddWithValue("@habilitado", t.Habilitado);
                    command.Parameters.AddWithValue("@id", t.IdTipoInmueble);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<TipoDeInmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var res = new List<TipoDeInmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"SELECT ID_TipoInmueble AS IdTipoInmueble, Nombre, Habilitado
                    FROM tipo_inmueble
                    ORDER BY Nombre
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new TipoDeInmueble
                        {
                            IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                            Nombre = reader.GetString("Nombre"),
                            Habilitado = reader.GetBoolean("Habilitado"),
                        });
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
                string sql = "SELECT COUNT(ID_TipoInmueble) FROM tipo_inmueble";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public TipoDeInmueble? ObtenerPorId(int id)
        {
            TipoDeInmueble? t = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT ID_TipoInmueble AS IdTipoInmueble, Nombre, Habilitado
                    FROM tipo_inmueble WHERE ID_TipoInmueble = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        t = new TipoDeInmueble
                        {
                            IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                            Nombre = reader.GetString("Nombre"),
                            Habilitado = reader.GetBoolean("Habilitado"),
                        };
                    }
                    connection.Close();
                }
            }
            return t;
        }

        public int ContarInmueblesQueLoUsan(int idTipoInmueble)
        {
            int res = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM inmueble WHERE ID_TipoInmueble = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idTipoInmueble);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }
    }
}