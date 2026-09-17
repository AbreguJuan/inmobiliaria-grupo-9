using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace inmobiliaria_grupo_9.Models
{
    public class RepositorioImagenInmueble : RepositorioBase, IRepositorioImagenInmueble
    {
        public RepositorioImagenInmueble(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(ImagenInmueble imagen)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO imageninmueble (ID_Inmueble, Url) VALUES (@idInmueble, @url);
            SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", imagen.IdInmueble);
                    command.Parameters.AddWithValue("@url", imagen.Url);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    imagen.IdImagen = res;
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int idImagen)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM imageninmueble WHERE ID_Imagen = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idImagen);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<ImagenInmueble> ObtenerPorInmueble(int idInmueble)
        {
            var res = new List<ImagenInmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT ID_Imagen AS IdImagen, ID_Inmueble AS IdInmueble, Url FROM imageninmueble WHERE ID_Inmueble = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idInmueble);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new ImagenInmueble
                        {
                            IdImagen = reader.GetInt32("IdImagen"),
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Url = reader.GetString("Url"),
                        });
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}