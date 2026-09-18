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
                    (ID_TipoInmueble, Provincia, Localidad, Direccion, PrecioXDia, Metros_Cuadrados, Nro_Ambientes, Nro_Banios, ID_Propietario, Habilitado, FotoPortada, Cupo, Latitud, Longitud)
                    VALUES (@tipo, @provincia, @localidad, @direccion, @precio, @metros, @ambientes, @banios, @idPropietario, @habilitado, @fotoPortada, @cupo, @latitud, @longitud);
                    SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@tipo", i.IdTipoInmueble);
                    command.Parameters.AddWithValue("@provincia", i.Provincia);
                    command.Parameters.AddWithValue("@localidad", i.Localidad);
                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@precio", i.PrecioXDia);
                    command.Parameters.AddWithValue("@porcentajeReserva", i.PorcentajeReserva);
                    command.Parameters.AddWithValue("@metros", i.MetrosCuadrados);
                    command.Parameters.AddWithValue("@ambientes", i.NroAmbientes);
                    command.Parameters.AddWithValue("@banios", i.NroBanios);
                    command.Parameters.AddWithValue("@idPropietario", i.IdPropietario);
                    command.Parameters.AddWithValue("@habilitado", i.Habilitado);
                    command.Parameters.AddWithValue("@fotoPortada", (object?)i.FotoPortada ?? DBNull.Value);
                    command.Parameters.AddWithValue("@cupo", i.Cupo);
                    command.Parameters.AddWithValue("@latitud", (object?)i.Latitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", (object?)i.Longitud ?? DBNull.Value);
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
                    ID_TipoInmueble=@idTipo, Provincia=@provincia, Localidad=@localidad, Direccion=@direccion,
                    PrecioXDia=@precio, PorcentajeReserva=@porcentajeReserva, Metros_Cuadrados=@metros, Nro_Ambientes=@ambientes,
                    Nro_Banios=@banios, ID_Propietario=@idPropietario, Habilitado=@habilitado, FotoPortada=@fotoPortada,
                    Cupo=@cupo, Latitud=@latitud, Longitud=@longitud
                    WHERE ID_Inmueble=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idTipo", i.IdTipoInmueble);
                    command.Parameters.AddWithValue("@provincia", i.Provincia);
                    command.Parameters.AddWithValue("@localidad", i.Localidad);
                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@precio", i.PrecioXDia);
                    command.Parameters.AddWithValue("@porcentajeReserva", i.PorcentajeReserva);
                    command.Parameters.AddWithValue("@metros", i.MetrosCuadrados);
                    command.Parameters.AddWithValue("@ambientes", i.NroAmbientes);
                    command.Parameters.AddWithValue("@banios", i.NroBanios);
                    command.Parameters.AddWithValue("@idPropietario", i.IdPropietario);
                    command.Parameters.AddWithValue("@habilitado", i.Habilitado);
                    command.Parameters.AddWithValue("@id", i.IdInmueble);
                    command.Parameters.AddWithValue("@fotoPortada", (object?)i.FotoPortada ?? DBNull.Value);
                    command.Parameters.AddWithValue("@cupo", i.Cupo);
                    command.Parameters.AddWithValue("@latitud", (object?)i.Latitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", (object?)i.Longitud ?? DBNull.Value);
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
                    SELECT i.ID_Inmueble AS IdInmueble, i.ID_TipoInmueble AS IdTipoInmueble, i.Provincia, i.Localidad, i.Direccion,
                        i.PrecioXDia, i.PorcentajeReserva, i.Metros_Cuadrados AS MetrosCuadrados, i.Nro_Ambientes AS NroAmbientes,
                        i.Nro_Banios AS NroBanios, i.ID_Propietario AS IdPropietario, i.Habilitado, i.FotoPortada,
                        i.Cupo, i.Latitud, i.Longitud,
                        p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario,
                        t.Nombre AS NombreTipo
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.ID_Propietario = p.ID_Propietario
                    INNER JOIN tipo_inmueble t ON i.ID_TipoInmueble = t.ID_TipoInmueble
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
                            IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                            TipoDeInmueble = new TipoDeInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                                Nombre = reader.GetString("NombreTipo")
                            },
                            Provincia = reader.GetString("Provincia"),
                            Localidad = reader.GetString("Localidad"),
                            Direccion = reader.GetString("Direccion"),
                            PrecioXDia = Convert.ToDecimal(reader.GetDouble("PrecioXDia")),
                            PorcentajeReserva = Convert.ToDecimal(reader["PorcentajeReserva"]),
                            MetrosCuadrados = Convert.ToDecimal(reader.GetInt32("MetrosCuadrados")),
                            NroAmbientes = reader.GetInt32("NroAmbientes"),
                            NroBanios = reader.GetInt32("NroBanios"),
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Habilitado = reader.GetBoolean("Habilitado"),
                            Propietario = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("NombrePropietario"),
                                Apellido = reader.GetString("ApellidoPropietario")
                            },
                            FotoPortada = reader.IsDBNull(reader.GetOrdinal("FotoPortada")) ? null : reader.GetString("FotoPortada"),
                            Cupo = reader.GetInt32("Cupo"),
                            Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? null : reader.GetDecimal("Latitud"),
                            Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? null : reader.GetDecimal("Longitud"),
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
                    SELECT i.ID_Inmueble AS IdInmueble, i.ID_TipoInmueble AS IdTipoInmueble, i.Provincia, i.Localidad, i.Direccion,
                        i.PrecioXDia, i.PorcentajeReserva, i.Metros_Cuadrados AS MetrosCuadrados, i.Nro_Ambientes AS NroAmbientes,
                        i.Nro_Banios AS NroBanios, i.ID_Propietario AS IdPropietario, i.Habilitado, i.FotoPortada,
                        i.Cupo, i.Latitud, i.Longitud,
                        p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario,
                        t.Nombre AS NombreTipo
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.ID_Propietario = p.ID_Propietario
                    INNER JOIN tipo_inmueble t ON i.ID_TipoInmueble = t.ID_TipoInmueble
                    WHERE i.ID_Inmueble = @id";
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
                            IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                            TipoDeInmueble = new TipoDeInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                                Nombre = reader.GetString("NombreTipo")
                            },
                            Provincia = reader.GetString("Provincia"),
                            Localidad = reader.GetString("Localidad"),
                            Direccion = reader.GetString("Direccion"),
                            PrecioXDia = Convert.ToDecimal(reader.GetDouble("PrecioXDia")),
                            PorcentajeReserva = Convert.ToDecimal(reader["PorcentajeReserva"]),
                            MetrosCuadrados = Convert.ToDecimal(reader.GetInt32("MetrosCuadrados")),
                            NroAmbientes = reader.GetInt32("NroAmbientes"),
                            NroBanios = reader.GetInt32("NroBanios"),
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Habilitado = reader.GetBoolean("Habilitado"),
                            Propietario = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("NombrePropietario"),
                                Apellido = reader.GetString("ApellidoPropietario")
                            },
                            FotoPortada = reader.IsDBNull(reader.GetOrdinal("FotoPortada")) ? null : reader.GetString("FotoPortada"),
                            Cupo = reader.GetInt32("Cupo"),
                            Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? null : reader.GetDecimal("Latitud"),
                            Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? null : reader.GetDecimal("Longitud"),
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
                    SELECT i.ID_Inmueble AS IdInmueble, i.ID_TipoInmueble AS IdTipoInmueble, i.Provincia, i.Localidad, i.Direccion,
                        i.PrecioXDia, i.PorcentajeReserva, i.Metros_Cuadrados AS MetrosCuadrados, i.Nro_Ambientes AS NroAmbientes,
                        i.Nro_Banios AS NroBanios, i.ID_Propietario AS IdPropietario, i.Habilitado, i.FotoPortada,
                        i.Cupo, i.Latitud, i.Longitud,
                        p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario,
                        t.Nombre AS NombreTipo
                    FROM Inmueble i
                    INNER JOIN Propietario p ON i.ID_Propietario = p.ID_Propietario
                    INNER JOIN tipo_inmueble t ON i.ID_TipoInmueble = t.ID_TipoInmueble
                    WHERE i.ID_Propietario = @idPropietario";
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
                            TipoDeInmueble = new TipoDeInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                                Nombre = reader.GetString("NombreTipo")
                            },
                            Provincia = reader.GetString("Provincia"),
                            Localidad = reader.GetString("Localidad"),
                            Direccion = reader.GetString("Direccion"),
                            PrecioXDia = Convert.ToDecimal(reader.GetDouble("PrecioXDia")),
                            PorcentajeReserva = Convert.ToDecimal(reader["PorcentajeReserva"]),
                            MetrosCuadrados = Convert.ToDecimal(reader.GetInt32("MetrosCuadrados")),
                            NroAmbientes = reader.GetInt32("NroAmbientes"),
                            NroBanios = reader.GetInt32("NroBanios"),
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Habilitado = reader.GetBoolean("Habilitado"),
                            Propietario = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("NombrePropietario"),
                                Apellido = reader.GetString("ApellidoPropietario")
                            },
                            FotoPortada = reader.IsDBNull(reader.GetOrdinal("FotoPortada")) ? null : reader.GetString("FotoPortada"),
                            Cupo = reader.GetInt32("Cupo"),
                            Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? null : reader.GetDecimal("Latitud"),
                            Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? null : reader.GetDecimal("Longitud"),
                        });
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inmueble> Buscar(string? texto, decimal? precio = null, string? operadorPrecio = null, bool? habilitado = null,
            int? ambientesMinimo = null, decimal? metrosMinimo = null, decimal? metrosMaximo = null,
            DateTime? disponibleDesde = null, DateTime? disponibleHasta = null,
            int? cupoMinimo = null, decimal? latitud = null, decimal? longitud = null, decimal? radioKm = null,
            int? idPropietario = null)
        {
            var res = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand { Connection = connection };
                var condiciones = new List<string>();

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    condiciones.Add(@"(i.Provincia LIKE @texto
                OR i.Localidad LIKE @texto
                OR i.Direccion LIKE @texto
                OR t.Nombre LIKE @texto
                OR p.Nombre LIKE @texto
                OR p.Apellido LIKE @texto
                OR CONCAT(p.Nombre, ' ', p.Apellido) LIKE @texto)");
                    command.Parameters.AddWithValue("@texto", $"%{texto}%");
                }

                if (precio.HasValue && !string.IsNullOrWhiteSpace(operadorPrecio))
                {
                    string operadorSql = operadorPrecio switch
                    {
                        "mayor" => ">",
                        "menor" => "<",
                        "igual" => "=",
                        _ => "="
                    };
                    condiciones.Add($"i.PrecioXDia {operadorSql} @precio");
                    command.Parameters.AddWithValue("@precio", precio.Value);
                }

                if (habilitado.HasValue)
                {
                    condiciones.Add("i.Habilitado = @habilitado");
                    command.Parameters.AddWithValue("@habilitado", habilitado.Value);
                }

                if (ambientesMinimo.HasValue)
                {
                    condiciones.Add("i.Nro_Ambientes >= @ambientesMinimo");
                    command.Parameters.AddWithValue("@ambientesMinimo", ambientesMinimo.Value);
                }

                if (metrosMinimo.HasValue)
                {
                    condiciones.Add("i.Metros_Cuadrados >= @metrosMinimo");
                    command.Parameters.AddWithValue("@metrosMinimo", metrosMinimo.Value);
                }

                if (metrosMaximo.HasValue)
                {
                    condiciones.Add("i.Metros_Cuadrados <= @metrosMaximo");
                    command.Parameters.AddWithValue("@metrosMaximo", metrosMaximo.Value);
                }
                if (cupoMinimo.HasValue)
                {
                    condiciones.Add("i.Cupo >= @cupoMinimo");
                    command.Parameters.AddWithValue("@cupoMinimo", cupoMinimo.Value);
                }

                if (idPropietario.HasValue)
                {
                    condiciones.Add("i.ID_Propietario = @idPropietarioFiltro");
                    command.Parameters.AddWithValue("@idPropietarioFiltro", idPropietario.Value);
                }
                if (latitud.HasValue && longitud.HasValue && radioKm.HasValue)
                {
                    // Fórmula de Haversine: distancia en km entre dos coordenadas sobre la Tierra
                    condiciones.Add(@"(6371 * ACOS(
        COS(RADIANS(@lat)) * COS(RADIANS(i.Latitud)) * COS(RADIANS(i.Longitud) - RADIANS(@lng))
        + SIN(RADIANS(@lat)) * SIN(RADIANS(i.Latitud))
    )) <= @radio");
                    command.Parameters.AddWithValue("@lat", latitud.Value);
                    command.Parameters.AddWithValue("@lng", longitud.Value);
                    command.Parameters.AddWithValue("@radio", radioKm.Value);
                }

                if (disponibleDesde.HasValue && disponibleHasta.HasValue)
                {
                    condiciones.Add(@"NOT EXISTS (
                        SELECT 1 FROM reserva r
                        WHERE r.ID_Inmueble = i.ID_Inmueble
                            AND r.Desde < @dispHasta
                            AND r.Hasta > @dispDesde
                    )");
                    command.Parameters.AddWithValue("@dispDesde", disponibleDesde.Value);
                    command.Parameters.AddWithValue("@dispHasta", disponibleHasta.Value);
                }

                string where = condiciones.Count > 0 ? "WHERE " + string.Join(" AND ", condiciones) : "";

                command.CommandText = $@"
            SELECT i.ID_Inmueble AS IdInmueble, i.ID_TipoInmueble AS IdTipoInmueble, i.Provincia, i.Localidad, i.Direccion,
               i.PrecioXDia, i.PorcentajeReserva, i.Metros_Cuadrados AS MetrosCuadrados, i.Nro_Ambientes AS NroAmbientes,
                i.Nro_Banios AS NroBanios, i.ID_Propietario AS IdPropietario, i.Habilitado, i.FotoPortada,
                i.Cupo, i.Latitud, i.Longitud,
                p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario,
                t.Nombre AS NombreTipo
            FROM Inmueble i
            INNER JOIN Propietario p ON i.ID_Propietario = p.ID_Propietario
            INNER JOIN tipo_inmueble t ON i.ID_TipoInmueble = t.ID_TipoInmueble
            {where}";

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    res.Add(new Inmueble
                    {
                        IdInmueble = reader.GetInt32("IdInmueble"),
                        IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                        TipoDeInmueble = new TipoDeInmueble
                        {
                            IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                            Nombre = reader.GetString("NombreTipo")
                        },
                        Provincia = reader.GetString("Provincia"),
                        Localidad = reader.GetString("Localidad"),
                        Direccion = reader.GetString("Direccion"),
                        PrecioXDia = Convert.ToDecimal(reader.GetDouble("PrecioXDia")),
                        PorcentajeReserva = Convert.ToDecimal(reader["PorcentajeReserva"]),
                        MetrosCuadrados = Convert.ToDecimal(reader.GetInt32("MetrosCuadrados")),
                        NroAmbientes = reader.GetInt32("NroAmbientes"),
                        NroBanios = reader.GetInt32("NroBanios"),
                        IdPropietario = reader.GetInt32("IdPropietario"),
                        Habilitado = reader.GetBoolean("Habilitado"),
                        Propietario = new Propietario
                        {
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Nombre = reader.GetString("NombrePropietario"),
                            Apellido = reader.GetString("ApellidoPropietario")
                        },
                        FotoPortada = reader.IsDBNull(reader.GetOrdinal("FotoPortada")) ? null : reader.GetString("FotoPortada"),
                        Cupo = reader.GetInt32("Cupo"),
                        Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? null : reader.GetDecimal("Latitud"),
                        Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? null : reader.GetDecimal("Longitud"),
                    });
                }
                connection.Close();
            }
            return res;
        }

        public IList<Inmueble> ObtenerSinReservas(int dias)
        {
            var res = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
            SELECT i.ID_Inmueble AS IdInmueble, i.ID_TipoInmueble AS IdTipoInmueble, i.Provincia, i.Localidad, i.Direccion,
                i.PrecioXDia, i.Metros_Cuadrados AS MetrosCuadrados, i.Nro_Ambientes AS NroAmbientes,
                i.Nro_Banios AS NroBanios, i.ID_Propietario AS IdPropietario, i.Habilitado, i.FotoPortada,
                i.Cupo, i.Latitud, i.Longitud,
                p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario,
                t.Nombre AS NombreTipo
            FROM Inmueble i
            INNER JOIN Propietario p ON i.ID_Propietario = p.ID_Propietario
            INNER JOIN tipo_inmueble t ON i.ID_TipoInmueble = t.ID_TipoInmueble
            WHERE NOT EXISTS (
                SELECT 1 FROM reserva r
                WHERE r.ID_Inmueble = i.ID_Inmueble
                  AND r.Hasta >= DATE_SUB(CURDATE(), INTERVAL {dias} DAY)
            )";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                            TipoDeInmueble = new TipoDeInmueble { Nombre = reader.GetString("NombreTipo") },
                            Provincia = reader.GetString("Provincia"),
                            Localidad = reader.GetString("Localidad"),
                            Direccion = reader.GetString("Direccion"),
                            PrecioXDia = Convert.ToDecimal(reader.GetDouble("PrecioXDia")),
                            MetrosCuadrados = Convert.ToDecimal(reader.GetInt32("MetrosCuadrados")),
                            NroAmbientes = reader.GetInt32("NroAmbientes"),
                            NroBanios = reader.GetInt32("NroBanios"),
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Habilitado = reader.GetBoolean("Habilitado"),
                            FotoPortada = reader.IsDBNull(reader.GetOrdinal("FotoPortada")) ? null : reader.GetString("FotoPortada"),
                            Cupo = reader.GetInt32("Cupo"),
                            Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? null : reader.GetDecimal("Latitud"),
                            Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? null : reader.GetDecimal("Longitud"),
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