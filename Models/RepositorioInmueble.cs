using System.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace inmobiliaria_grupo_9.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Inmueble i)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Inmueble 
                    (Tipo, Provincia, Localidad, Direccion, PrecioXDia, Metros_Cuadrados, Nro_Ambientes, Nro_Banios, ID_Propietario, Habilitado)
                    VALUES (@tipo, @provincia, @localidad, @direccion, @precio, @metros, @ambientes, @banios, @idPropietario, @habilitado);
                    SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@tipo", i.Tipo);
                    command.Parameters.AddWithValue("@provincia", i.Provincia);
                    command.Parameters.AddWithValue("@localidad", i.Localidad);
                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@precio", i.PrecioXDia);
                    command.Parameters.AddWithValue("@metros", i.MetrosCuadrados);
                    command.Parameters.AddWithValue("@ambientes", i.NroAmbientes);
                    command.Parameters.AddWithValue("@banios", i.NroBanios);
                    command.Parameters.AddWithValue("@idPropietario", i.IdPropietario);
                    command.Parameters.AddWithValue("@habilitado", i.Habilitado);
                    connection.Open();
                    res = System.Convert.ToInt32(command.ExecuteScalar());
                    i.IdInmueble = res;
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
                string sql = "DELETE FROM Inmueble WHERE ID_Inmueble = @id";
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

        public int Modificacion(Inmueble i)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Inmueble SET
                    Tipo=@tipo, Provincia=@provincia, Localidad=@localidad, Direccion=@direccion,
                    PrecioXDia=@precio, Metros_Cuadrados=@metros, Nro_Ambientes=@ambientes,
                    Nro_Banios=@banios, ID_Propietario=@idPropietario, Habilitado=@habilitado
                    WHERE ID_Inmueble=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tipo", i.Tipo);
                    command.Parameters.AddWithValue("@provincia", i.Provincia);
                    command.Parameters.AddWithValue("@localidad", i.Localidad);
                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@precio", i.PrecioXDia);
                    command.Parameters.AddWithValue("@metros", i.MetrosCuadrados);
                    command.Parameters.AddWithValue("@ambientes", i.NroAmbientes);
                    command.Parameters.AddWithValue("@banios", i.NroBanios);
                    command.Parameters.AddWithValue("@idPropietario", i.IdPropietario);
                    command.Parameters.AddWithValue("@habilitado", i.Habilitado);
                    command.Parameters.AddWithValue("@id", i.IdInmueble);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var res = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT i.ID_Inmueble AS IdInmueble, i.Tipo, i.Provincia, i.Localidad, i.Direccion,
                           i.PrecioXDia, i.Metros_Cuadrados AS MetrosCuadrados, i.Nro_Ambientes AS NroAmbientes,
                           i.Nro_Banios AS NroBanios, i.ID_Propietario AS IdPropietario, i.Habilitado,
                           p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.ID_Propietario = p.ID_Propietario
                    ORDER BY i.ID_Inmueble
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Tipo = reader.GetString("Tipo"),
                            Provincia = reader.GetString("Provincia"),
                            Localidad = reader.GetString("Localidad"),
                            Direccion = reader.GetString("Direccion"),
                            PrecioXDia = reader.GetDecimal("PrecioXDia"),
                            MetrosCuadrados = reader.GetDecimal("MetrosCuadrados"),
                            NroAmbientes = reader.GetInt32("NroAmbientes"),
                            NroBanios = reader.GetInt32("NroBanios"),
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Habilitado = reader.GetBoolean("Habilitado"),
                            Propietario = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("NombrePropietario"),
                                Apellido = reader.GetString("ApellidoPropietario")
                            }
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
                string sql = "SELECT COUNT(ID_Inmueble) FROM Inmueble";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    res = System.Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? i = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT i.ID_Inmueble AS IdInmueble, i.Tipo, i.Provincia, i.Localidad, i.Direccion,
                           i.PrecioXDia, i.Metros_Cuadrados AS MetrosCuadrados, i.Nro_Ambientes AS NroAmbientes,
                           i.Nro_Banios AS NroBanios, i.ID_Propietario AS IdPropietario, i.Habilitado,
                           p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.ID_Propietario = p.ID_Propietario 
                    WHERE i.ID_Inmueble=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        i = new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Tipo = reader.GetString("Tipo"),
                            Provincia = reader.GetString("Provincia"),
                            Localidad = reader.GetString("Localidad"),
                            Direccion = reader.GetString("Direccion"),
                            PrecioXDia = reader.GetDecimal("PrecioXDia"),
                            MetrosCuadrados = reader.GetDecimal("MetrosCuadrados"),
                            NroAmbientes = reader.GetInt32("NroAmbientes"),
                            NroBanios = reader.GetInt32("NroBanios"),
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Habilitado = reader.GetBoolean("Habilitado"),
                            Propietario = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("NombrePropietario"),
                                Apellido = reader.GetString("ApellidoPropietario")
                            }
                        };
                    }
                    connection.Close();
                }
            }
            return i;
        }

        public IList<Inmueble> BuscarPorPropietario(int idPropietario)
        {
            var res = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT i.ID_Inmueble AS IdInmueble, i.Tipo, i.Provincia, i.Localidad, i.Direccion,
                        i.PrecioXDia, i.Metros_Cuadrados AS MetrosCuadrados, i.Nro_Ambientes AS NroAmbientes,
                        i.Nro_Banios AS NroBanios, i.ID_Propietario AS IdPropietario, i.Habilitado,
                        p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.ID_Propietario = p.ID_Propietario 
                    WHERE i.ID_Propietario=@idPropietario";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idPropietario", idPropietario);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Tipo = reader.GetString("Tipo"),
                            Provincia = reader.GetString("Provincia"),
                            Localidad = reader.GetString("Localidad"),
                            Direccion = reader.GetString("Direccion"),
                            PrecioXDia = reader.GetDecimal("PrecioXDia"),
                            MetrosCuadrados = reader.GetDecimal("MetrosCuadrados"),
                            NroAmbientes = reader.GetInt32("NroAmbientes"),
                            NroBanios = reader.GetInt32("NroBanios"),
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Habilitado = reader.GetBoolean("Habilitado"),
                            Propietario = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("NombrePropietario"),
                                Apellido = reader.GetString("ApellidoPropietario")
                            }
                        });
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}