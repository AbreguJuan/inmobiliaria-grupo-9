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
    (ID_Inquilino, ID_Inmueble, Desde, Hasta, MontoDiario)
    VALUES (@idInquilino, @idInmueble, @desde, @hasta, @montoDiario);
    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
                    command.Parameters.AddWithValue("@desde", r.Desde);
                    command.Parameters.AddWithValue("@hasta", r.Hasta);
                    command.Parameters.AddWithValue("@montoDiario", r.MontoDiario);

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
                            FechaFinalizacion = reader.IsDBNull(reader.GetOrdinal("FechaFinalizacion"))
                            ? null
                        : reader.GetDateTime("FechaFinalizacion"),

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

                        i.Nombre AS NombreInquilino,
                        i.Apellido AS ApellidoInquilino,

                        inm.ID_TipoInmueble AS IdTipoInmuebleInmueble,
                        t.Nombre AS TipoInmueble,
                        inm.Direccion AS DireccionInmueble

                    FROM reserva r
                INNER JOIN inquilino i ON r.ID_Inquilino = i.ID_Inquilino
                INNER JOIN inmueble inm ON r.ID_Inmueble = inm.ID_Inmueble
                INNER JOIN tipo_inmueble t ON inm.ID_TipoInmueble = t.ID_TipoInmueble

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
                            FechaFinalizacion = reader.IsDBNull(reader.GetOrdinal("FechaFinalizacion"))
                            ? null
                            : reader.GetDateTime("FechaFinalizacion"),

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

                    connection.Close();
                }
            }

            return r;
        }

        // Verifica que no se genere una nueva reserva
        // si el inmueble ya está reservado en esas fechas.
        public bool ExisteSuperposicion(
            int idInmueble,
            DateTime desde,
            DateTime hasta,
            int idReservaExcluida = 0)
        {
            bool existe = false;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT COUNT(*)
                    FROM reserva
                    WHERE ID_Inmueble = @idInmueble
                    AND ID_Reserva <> @idExcluir
                    AND Desde < @hasta
                    AND Hasta > @desde";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    command.Parameters.AddWithValue("@idExcluir", idReservaExcluida);
                    command.Parameters.AddWithValue("@desde", desde);
                    command.Parameters.AddWithValue("@hasta", hasta);

                    connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar());

                    existe = cantidad > 0;

                    connection.Close();
                }
            }

            return existe;
        }
        public int FinalizarReserva(int idReserva, DateTime fechaFinalizacion)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE reserva
                       SET Finalizada = 1,
                           FechaFinalizacion = @fechaFinalizacion
                       WHERE ID_Reserva = @idReserva";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaFinalizacion", fechaFinalizacion);
                    command.Parameters.AddWithValue("@idReserva", idReserva);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return res;
        }

        public int RenovarReserva(int idReserva, DateTime nuevaFechaHasta)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE reserva
                       SET Hasta = @nuevaFechaHasta
                       WHERE ID_Reserva = @idReserva
                       AND Finalizada = 0";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nuevaFechaHasta", nuevaFechaHasta);
                    command.Parameters.AddWithValue("@idReserva", idReserva);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }

            return res;
        }
    }
}


