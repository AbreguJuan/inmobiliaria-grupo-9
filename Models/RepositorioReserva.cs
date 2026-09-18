using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace inmobiliaria_grupo_9.Models
{
    public class RepositorioReserva : RepositorioBase, IRepositorioReserva
    {
        public RepositorioReserva(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Reserva r)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO reserva
                    (ID_Inquilino, ID_Inmueble, Desde, Hasta, MontoDiario, CreadoPor)
                    VALUES (@idInquilino, @idInmueble, @desde, @hasta, @montoDiario, @creadoPor);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
                    command.Parameters.AddWithValue("@desde", r.Desde);
                    command.Parameters.AddWithValue("@hasta", r.Hasta);
                    command.Parameters.AddWithValue("@montoDiario", r.MontoDiario);
                    command.Parameters.AddWithValue("@creadoPor", r.CreadoPor ?? (object)DBNull.Value);

                    connection.Open();

                    res = Convert.ToInt32(command.ExecuteScalar());

                    r.IdReserva = res;

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
                string sql = "DELETE FROM reserva WHERE ID_Reserva = @id";

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

        public int Modificacion(Reserva r)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE reserva SET
                    ID_Inquilino = @idInquilino,
                    ID_Inmueble = @idInmueble,
                    Desde = @desde,
                    Hasta = @hasta
                    WHERE ID_Reserva = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
                    command.Parameters.AddWithValue("@desde", r.Desde);
                    command.Parameters.AddWithValue("@hasta", r.Hasta);
                    command.Parameters.AddWithValue("@id", r.IdReserva);

                    connection.Open();

                    res = command.ExecuteNonQuery();

                    connection.Close();
                }
            }

            return res;
        }

        public IList<Reserva> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var res = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
                    SELECT
                        r.ID_Reserva AS IdReserva,
                        r.ID_Inquilino AS IdInquilino,
                        r.ID_Inmueble AS IdInmueble,
                        r.Desde,
                        r.Hasta,
                        r.MontoDiario,
                        r.Finalizada,
                        r.FechaFinalizacion,
                        r.CreadoPor,
                        r.TerminadoPor,

                        i.Nombre AS NombreInquilino,
                        i.Apellido AS ApellidoInquilino,

                        inm.ID_TipoInmueble AS IdTipoInmuebleInmueble,
                        t.Nombre AS TipoInmueble,
                        inm.Direccion AS DireccionInmueble

                    FROM reserva r
                    INNER JOIN inquilino i ON r.ID_Inquilino = i.ID_Inquilino
                    INNER JOIN inmueble inm ON r.ID_Inmueble = inm.ID_Inmueble
                    INNER JOIN tipo_inmueble t ON inm.ID_TipoInmueble = t.ID_TipoInmueble

                    ORDER BY r.ID_Reserva

                    LIMIT {tamPagina}
                    OFFSET {(paginaNro - 1) * tamPagina}";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        res.Add(new Reserva
                        {
                            IdReserva = reader.GetInt32("IdReserva"),
                            IdInquilino = reader.GetInt32("IdInquilino"),
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Desde = reader.GetDateTime("Desde"),
                            Hasta = reader.GetDateTime("Hasta"),
                            MontoDiario = reader.GetDecimal("MontoDiario"),
                            Finalizada = reader.GetBoolean("Finalizada"),
                            FechaFinalizacion = reader.IsDBNull(reader.GetOrdinal("FechaFinalizacion")) ? null : reader.GetDateTime("FechaFinalizacion"),
                            CreadoPor = reader.IsDBNull(reader.GetOrdinal("CreadoPor")) ? null : reader.GetInt32("CreadoPor"),
                            TerminadoPor = reader.IsDBNull(reader.GetOrdinal("TerminadoPor")) ? null : reader.GetInt32("TerminadoPor"),

                            Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Nombre = reader.GetString("NombreInquilino"),
                                Apellido = reader.GetString("ApellidoInquilino")
                            },

                            Inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                IdTipoInmueble = reader.GetInt32("IdTipoInmuebleInmueble"),
                                TipoDeInmueble = new TipoDeInmueble { Nombre = reader.GetString("TipoInmueble") },
                                Direccion = reader.GetString("DireccionInmueble")
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
                string sql = "SELECT COUNT(ID_Reserva) FROM reserva";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }

            return res;
        }

        public Reserva? ObtenerPorId(int id)
        {
            Reserva? r = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT
                        r.ID_Reserva AS IdReserva,
                        r.ID_Inquilino AS IdInquilino,
                        r.ID_Inmueble AS IdInmueble,
                        r.Desde,
                        r.Hasta,
                        r.MontoDiario,
                        r.Finalizada,
                        r.FechaFinalizacion,
                        r.CreadoPor,
                        r.TerminadoPor,

                        i.Nombre AS NombreInquilino,
                        i.Apellido AS ApellidoInquilino,

                        inm.ID_TipoInmueble AS IdTipoInmuebleInmueble,
                        t.Nombre AS TipoInmueble,
                        inm.Direccion AS DireccionInmueble,
                        
                        uc.Nombre AS CreadorNombre,
                        uc.Apellido AS CreadorApellido,
                        ut.Nombre AS TerminadorNombre,
                        ut.Apellido AS TerminadorApellido

                    FROM reserva r
                    INNER JOIN inquilino i ON r.ID_Inquilino = i.ID_Inquilino
                    INNER JOIN inmueble inm ON r.ID_Inmueble = inm.ID_Inmueble
                    INNER JOIN tipo_inmueble t ON inm.ID_TipoInmueble = t.ID_TipoInmueble
                    LEFT JOIN usuario uc ON r.CreadoPor = uc.ID_Usuario
                    LEFT JOIN usuario ut ON r.TerminadoPor = ut.ID_Usuario

                    WHERE r.ID_Reserva = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        r = new Reserva
                        {
                            IdReserva = reader.GetInt32("IdReserva"),
                            IdInquilino = reader.GetInt32("IdInquilino"),
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Desde = reader.GetDateTime("Desde"),
                            Hasta = reader.GetDateTime("Hasta"),
                            MontoDiario = reader.GetDecimal("MontoDiario"),
                            Finalizada = reader.GetBoolean("Finalizada"),
                            FechaFinalizacion = reader.IsDBNull(reader.GetOrdinal("FechaFinalizacion")) ? null : reader.GetDateTime("FechaFinalizacion"),
                            CreadoPor = reader.IsDBNull(reader.GetOrdinal("CreadoPor")) ? null : reader.GetInt32("CreadoPor"),
                            TerminadoPor = reader.IsDBNull(reader.GetOrdinal("TerminadoPor")) ? null : reader.GetInt32("TerminadoPor"),

                            Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Nombre = reader.GetString("NombreInquilino"),
                                Apellido = reader.GetString("ApellidoInquilino")
                            },

                            Inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                IdTipoInmueble = reader.GetInt32("IdTipoInmuebleInmueble"),
                                TipoDeInmueble = new TipoDeInmueble { Nombre = reader.GetString("TipoInmueble") },
                                Direccion = reader.GetString("DireccionInmueble")
                            }
                        };

                        // Mapeo de la auditoría (Usuario Creador)
                        if (!reader.IsDBNull(reader.GetOrdinal("CreadoPor")))
                        {
                            r.Creador = new Usuario
                            {
                                Nombre = reader.GetString("CreadorNombre"),
                                Apellido = reader.GetString("CreadorApellido")
                            };
                        }

                        // Mapeo de la auditoría (Usuario Terminador)
                        if (!reader.IsDBNull(reader.GetOrdinal("TerminadoPor")))
                        {
                            r.Terminador = new Usuario
                            {
                                Nombre = reader.GetString("TerminadorNombre"),
                                Apellido = reader.GetString("TerminadorApellido")
                            };
                        }
                    }

                    connection.Close();
                }
            }

            return r;
        }

        public bool ExisteSuperposicion(
     int idInmueble,
     DateTime desde,
     DateTime hasta,
     int idReservaExcluida = 0)
        {
            bool existe = false;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT COUNT(*)
            FROM reserva
            WHERE ID_Inmueble = @idInmueble
            AND ID_Reserva <> @idExcluir
            AND Desde < @hasta
            AND (
                CASE
                    WHEN Finalizada = 1
                         AND FechaFinalizacion IS NOT NULL
                    THEN FechaFinalizacion
                    ELSE Hasta
                END
            ) > @desde;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    command.Parameters.AddWithValue("@idExcluir", idReservaExcluida);
                    command.Parameters.AddWithValue("@desde", desde);
                    command.Parameters.AddWithValue("@hasta", hasta);

                    connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar());

                    existe = cantidad > 0;
                }
            }

            return existe;
        }

        public int FinalizarReserva(int idReserva, DateTime fechaFinalizacion, int idUsuario)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE reserva
                       SET Finalizada = 1,
                           FechaFinalizacion = @fechaFinalizacion,
                           TerminadoPor = @idUsuario
                       WHERE ID_Reserva = @idReserva";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaFinalizacion", fechaFinalizacion);
                    command.Parameters.AddWithValue("@idReserva", idReserva);
                    command.Parameters.AddWithValue("@idUsuario", idUsuario);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return res;
        }


        public IList<Reserva> ObtenerPorInmueble(int idInmueble)
        {
            var res = new List<Reserva>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT ID_Reserva AS IdReserva, Desde, Hasta, Finalizada
            FROM reserva
            WHERE ID_Inmueble = @idInmueble";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new Reserva
                        {
                            IdReserva = reader.GetInt32("IdReserva"),
                            Desde = reader.GetDateTime("Desde"),
                            Hasta = reader.GetDateTime("Hasta"),
                            Finalizada = reader.GetBoolean("Finalizada"),
                        });
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<InmuebleConteo> ObtenerMasReservados(int dias = 365, int top = 10)
        {
            var res = new List<InmuebleConteo>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT r.ID_Inmueble AS IdInmueble, inm.Direccion, t.Nombre AS TipoNombre, COUNT(*) AS CantidadReservas
            FROM reserva r
            INNER JOIN inmueble inm ON r.ID_Inmueble = inm.ID_Inmueble
            INNER JOIN tipo_inmueble t ON inm.ID_TipoInmueble = t.ID_TipoInmueble
            WHERE r.Desde >= DATE_SUB(CURDATE(), INTERVAL @dias DAY)
            GROUP BY r.ID_Inmueble, inm.Direccion, t.Nombre
            ORDER BY CantidadReservas DESC
            LIMIT @top";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dias", dias);
                    command.Parameters.AddWithValue("@top", top);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new InmuebleConteo
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Direccion = reader.GetString("Direccion"),
                            TipoNombre = reader.GetString("TipoNombre"),
                            CantidadReservas = reader.GetInt32("CantidadReservas"),
                        });
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Reserva> ObtenerVigentes()
        {
            var res = new List<Reserva>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT r.ID_Reserva AS IdReserva, r.ID_Inquilino AS IdInquilino, r.ID_Inmueble AS IdInmueble,
                r.Desde, r.Hasta, r.MontoDiario, r.Finalizada, r.FechaFinalizacion,
                i.Nombre AS NombreInquilino, i.Apellido AS ApellidoInquilino,
                inm.ID_TipoInmueble AS IdTipoInmuebleInmueble, t.Nombre AS TipoInmueble, inm.Direccion AS DireccionInmueble
            FROM reserva r
            INNER JOIN inquilino i ON r.ID_Inquilino = i.ID_Inquilino
            INNER JOIN inmueble inm ON r.ID_Inmueble = inm.ID_Inmueble
            INNER JOIN tipo_inmueble t ON inm.ID_TipoInmueble = t.ID_TipoInmueble
            WHERE r.Finalizada = 0
              AND CURDATE() BETWEEN r.Desde AND r.Hasta";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearReserva(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Reserva> ObtenerPorVencer(int dias)
        {
            var res = new List<Reserva>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT r.ID_Reserva AS IdReserva, r.ID_Inquilino AS IdInquilino, r.ID_Inmueble AS IdInmueble,
                r.Desde, r.Hasta, r.MontoDiario, r.Finalizada, r.FechaFinalizacion,
                i.Nombre AS NombreInquilino, i.Apellido AS ApellidoInquilino,
                inm.ID_TipoInmueble AS IdTipoInmuebleInmueble, t.Nombre AS TipoInmueble, inm.Direccion AS DireccionInmueble
            FROM reserva r
            INNER JOIN inquilino i ON r.ID_Inquilino = i.ID_Inquilino
            INNER JOIN inmueble inm ON r.ID_Inmueble = inm.ID_Inmueble
            INNER JOIN tipo_inmueble t ON inm.ID_TipoInmueble = t.ID_TipoInmueble
            WHERE r.Finalizada = 0
              AND r.Hasta BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @dias DAY)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dias", dias);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearReserva(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        // Método auxiliar para no repetir el mapeo en los 2 de arriba
        private Reserva MapearReserva(MySqlDataReader reader)
        {
            return new Reserva
            {
                IdReserva = reader.GetInt32("IdReserva"),
                IdInquilino = reader.GetInt32("IdInquilino"),
                IdInmueble = reader.GetInt32("IdInmueble"),
                Desde = reader.GetDateTime("Desde"),
                Hasta = reader.GetDateTime("Hasta"),
                MontoDiario = reader.GetDecimal("MontoDiario"),
                Finalizada = reader.GetBoolean("Finalizada"),
                FechaFinalizacion = reader.IsDBNull(reader.GetOrdinal("FechaFinalizacion")) ? null : reader.GetDateTime("FechaFinalizacion"),
                Inquilino = new Inquilino
                {
                    IdInquilino = reader.GetInt32("IdInquilino"),
                    Nombre = reader.GetString("NombreInquilino"),
                    Apellido = reader.GetString("ApellidoInquilino")
                },
                Inmueble = new Inmueble
                {
                    IdInmueble = reader.GetInt32("IdInmueble"),
                    IdTipoInmueble = reader.GetInt32("IdTipoInmuebleInmueble"),
                    TipoDeInmueble = new TipoDeInmueble { Nombre = reader.GetString("TipoInmueble") },
                    Direccion = reader.GetString("DireccionInmueble")
                }
            };
        }

        public IList<Reserva> Buscar(string? inquilino = null, string? inmueble = null,
    DateTime? fechaDesde = null, DateTime? fechaHasta = null, bool? finalizada = null)
        {
            var res = new List<Reserva>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand { Connection = connection };
                var condiciones = new List<string>();

                if (!string.IsNullOrWhiteSpace(inquilino))
                {
                    condiciones.Add("(i.Nombre LIKE @inquilino OR i.Apellido LIKE @inquilino)");
                    command.Parameters.AddWithValue("@inquilino", $"%{inquilino}%");
                }

                if (!string.IsNullOrWhiteSpace(inmueble))
                {
                    condiciones.Add("(inm.Direccion LIKE @inmueble OR t.Nombre LIKE @inmueble)");
                    command.Parameters.AddWithValue("@inmueble", $"%{inmueble}%");
                }

                if (fechaDesde.HasValue)
                {
                    condiciones.Add("r.Desde >= @fechaDesde");
                    command.Parameters.AddWithValue("@fechaDesde", fechaDesde.Value);
                }

                if (fechaHasta.HasValue)
                {
                    condiciones.Add("r.Hasta <= @fechaHasta");
                    command.Parameters.AddWithValue("@fechaHasta", fechaHasta.Value);
                }

                if (finalizada.HasValue)
                {
                    condiciones.Add("r.Finalizada = @finalizada");
                    command.Parameters.AddWithValue("@finalizada", finalizada.Value);
                }

                string where = condiciones.Count > 0 ? "WHERE " + string.Join(" AND ", condiciones) : "";

                command.CommandText = $@"
            SELECT r.ID_Reserva AS IdReserva, r.ID_Inquilino AS IdInquilino, r.ID_Inmueble AS IdInmueble,
                r.Desde, r.Hasta, r.MontoDiario, r.Finalizada, r.FechaFinalizacion,
                i.Nombre AS NombreInquilino, i.Apellido AS ApellidoInquilino,
                inm.ID_TipoInmueble AS IdTipoInmuebleInmueble, t.Nombre AS TipoInmueble, inm.Direccion AS DireccionInmueble
            FROM reserva r
            INNER JOIN inquilino i ON r.ID_Inquilino = i.ID_Inquilino
            INNER JOIN inmueble inm ON r.ID_Inmueble = inm.ID_Inmueble
            INNER JOIN tipo_inmueble t ON inm.ID_TipoInmueble = t.ID_TipoInmueble
            {where}
            ORDER BY r.ID_Reserva DESC";

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    res.Add(MapearReserva(reader));
                }
                connection.Close();
            }
            return res;
        }
    }
}