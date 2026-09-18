using System.Data;
using MySqlConnector;

namespace inmobiliaria_grupo_9.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration)
            : base(configuration)
        {
        }

        public int Alta(Pago pago)
        {
            int res = -1;

            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                INSERT INTO pago
                (IdReserva, Concepto, FechaPago, Importe, Anulado, CreadoPor)
                VALUES
                (@IdReserva, @Concepto, @FechaPago, @Importe, 0, @CreadoPor);

                SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@IdReserva", pago.IdReserva);
            command.Parameters.AddWithValue("@Concepto", pago.Concepto);
            command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
            command.Parameters.AddWithValue("@Importe", pago.Importe);
            command.Parameters.AddWithValue("@CreadoPor", pago.CreadoPor ?? (object)DBNull.Value);

            connection.Open();

            res = Convert.ToInt32(command.ExecuteScalar());

            return res;
        }

        public int Modificacion(Pago pago)
        {
            int res = -1;

            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                UPDATE pago
                SET Concepto = @Concepto
                WHERE IdPago = @IdPago;";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Concepto", pago.Concepto);
            command.Parameters.AddWithValue("@IdPago", pago.IdPago);

            connection.Open();

            res = command.ExecuteNonQuery();

            return res;
        }

        public int Anular(int idPago, int idUsuario)
        {
            int res = -1;

            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                UPDATE pago
                SET Anulado = 1,
                    AnuladoPor = @idUsuario
                WHERE IdPago = @IdPago;";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@IdPago", idPago);
            command.Parameters.AddWithValue("@idUsuario", idUsuario);

            connection.Open();

            res = command.ExecuteNonQuery();

            return res;
        }

        public IList<Pago> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = new List<Pago>();

            using var connection = new MySqlConnection(connectionString);

            string sql = @"
        SELECT
            p.IdPago,
            p.IdReserva,
            p.Concepto,
            p.FechaPago,
            p.Importe,
            p.Anulado,
            p.CreadoPor,
            p.AnuladoPor,
            i.ID_Inquilino,
            i.Nombre AS InquilinoNombre,
            i.Apellido AS InquilinoApellido
        FROM pago p
        INNER JOIN reserva r ON p.IdReserva = r.ID_Reserva
        INNER JOIN inquilino i ON r.ID_Inquilino = i.ID_Inquilino
        ORDER BY p.FechaPago DESC
        LIMIT @tamPagina OFFSET @offset;";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@tamPagina", tamPagina);
            command.Parameters.AddWithValue(
                "@offset",
                (paginaNro - 1) * tamPagina
            );

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Pago
                {
                    IdPago = reader.GetInt32("IdPago"),
                    IdReserva = reader.GetInt32("IdReserva"),
                    Concepto = reader.GetString("Concepto"),
                    FechaPago = reader.GetDateTime("FechaPago"),
                    Importe = reader.GetDecimal("Importe"),
                    Anulado = reader.GetBoolean("Anulado"),

                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("CreadoPor"))
                        ? null
                        : reader.GetInt32("CreadoPor"),

                    AnuladoPor = reader.IsDBNull(reader.GetOrdinal("AnuladoPor"))
                        ? null
                        : reader.GetInt32("AnuladoPor"),

                    Reserva = new Reserva
                    {
                        IdReserva = reader.GetInt32("IdReserva"),

                        Inquilino = new Inquilino
                        {
                            IdInquilino = reader.GetInt32("ID_Inquilino"),
                            Nombre = reader.GetString("InquilinoNombre"),
                            Apellido = reader.GetString("InquilinoApellido")
                        }
                    }
                });
            }

            return lista;
        }
        public int ObtenerCantidad()
        {
            using var connection =
                new MySqlConnection(connectionString);

            string sql = "SELECT COUNT(IdPago) FROM pago;";

            using var command =
                new MySqlCommand(sql, connection);

            connection.Open();

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public Pago? ObtenerPorId(int idPago)
        {
            Pago? pago = null;

            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                SELECT
                    p.IdPago, p.IdReserva, p.Concepto, p.FechaPago, p.Importe, p.Anulado, p.CreadoPor, p.AnuladoPor,
                    uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido,
                    ua.Nombre AS AnuladorNombre, ua.Apellido AS AnuladorApellido
                FROM pago p
                LEFT JOIN usuario uc ON p.CreadoPor = uc.ID_Usuario
                LEFT JOIN usuario ua ON p.AnuladoPor = ua.ID_Usuario
                WHERE p.IdPago = @IdPago;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@IdPago", idPago);
            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                pago = new Pago
                {
                    IdPago = reader.GetInt32("IdPago"),
                    IdReserva = reader.GetInt32("IdReserva"),
                    Concepto = reader.GetString("Concepto"),
                    FechaPago = reader.GetDateTime("FechaPago"),
                    Importe = reader.GetDecimal("Importe"),
                    Anulado = reader.GetBoolean("Anulado"),
                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("CreadoPor")) ? null : reader.GetInt32("CreadoPor"),
                    AnuladoPor = reader.IsDBNull(reader.GetOrdinal("AnuladoPor")) ? null : reader.GetInt32("AnuladoPor")
                };

                if (!reader.IsDBNull(reader.GetOrdinal("CreadoPor")))
                {
                    pago.Creador = new Usuario
                    {
                        Nombre = reader.GetString("CreadorNombre"),
                        Apellido = reader.GetString("CreadorApellido")
                    };
                }

                if (!reader.IsDBNull(reader.GetOrdinal("AnuladoPor")))
                {
                    pago.Anulador = new Usuario
                    {
                        Nombre = reader.GetString("AnuladorNombre"),
                        Apellido = reader.GetString("AnuladorApellido")
                    };
                }
            }

            return pago;
        }

        public IList<Pago> ObtenerPorReserva(int idReserva)
        {
            var lista = new List<Pago>();

            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                SELECT
                    IdPago, IdReserva, Concepto, FechaPago, Importe, Anulado, CreadoPor, AnuladoPor
                FROM pago
                WHERE IdReserva = @IdReserva
                ORDER BY FechaPago DESC;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@IdReserva", idReserva);
            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Pago
                {
                    IdPago = reader.GetInt32("IdPago"),
                    IdReserva = reader.GetInt32("IdReserva"),
                    Concepto = reader.GetString("Concepto"),
                    FechaPago = reader.GetDateTime("FechaPago"),
                    Importe = reader.GetDecimal("Importe"),
                    Anulado = reader.GetBoolean("Anulado"),
                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("CreadoPor")) ? null : reader.GetInt32("CreadoPor"),
                    AnuladoPor = reader.IsDBNull(reader.GetOrdinal("AnuladoPor")) ? null : reader.GetInt32("AnuladoPor")
                });
            }

            return lista;
        }

        public IList<Pago> Buscar(string? concepto = null, decimal? importeMin = null, decimal? importeMax = null,
            DateTime? fechaDesde = null, DateTime? fechaHasta = null, bool? anulado = null, string? inquilino = null)
        {
            var lista = new List<Pago>();

            using var connection = new MySqlConnection(connectionString);
            var command = new MySqlCommand { Connection = connection };
            var condiciones = new List<string>();

            if (!string.IsNullOrWhiteSpace(concepto))
            {
                condiciones.Add("p.Concepto LIKE @concepto");
                command.Parameters.AddWithValue("@concepto", $"%{concepto}%");
            }

            if (importeMin.HasValue)
            {
                condiciones.Add("p.Importe >= @importeMin");
                command.Parameters.AddWithValue("@importeMin", importeMin.Value);
            }

            if (importeMax.HasValue)
            {
                condiciones.Add("p.Importe <= @importeMax");
                command.Parameters.AddWithValue("@importeMax", importeMax.Value);
            }

            if (fechaDesde.HasValue)
            {
                condiciones.Add("p.FechaPago >= @fechaDesde");
                command.Parameters.AddWithValue("@fechaDesde", fechaDesde.Value);
            }

            if (fechaHasta.HasValue)
            {
                condiciones.Add("p.FechaPago <= @fechaHasta");
                command.Parameters.AddWithValue("@fechaHasta", fechaHasta.Value);
            }

            if (anulado.HasValue)
            {
                condiciones.Add("p.Anulado = @anulado");
                command.Parameters.AddWithValue("@anulado", anulado.Value);
            }

            if (!string.IsNullOrWhiteSpace(inquilino))
            {
                condiciones.Add("(i.Nombre LIKE @inquilino OR i.Apellido LIKE @inquilino)");
                command.Parameters.AddWithValue("@inquilino", $"%{inquilino}%");
            }

            string where = condiciones.Count > 0 ? "WHERE " + string.Join(" AND ", condiciones) : "";

            command.CommandText = $@"
            SELECT p.IdPago, p.IdReserva, p.Concepto, p.FechaPago, p.Importe, p.Anulado, p.CreadoPor, p.AnuladoPor,
                i.ID_Inquilino, i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido
            FROM pago p
            INNER JOIN reserva r ON p.IdReserva = r.ID_Reserva
            INNER JOIN inquilino i ON r.ID_Inquilino = i.ID_Inquilino
            {where}
            ORDER BY p.FechaPago DESC";

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Pago
                {
                    IdPago = reader.GetInt32("IdPago"),
                    IdReserva = reader.GetInt32("IdReserva"),
                    Concepto = reader.GetString("Concepto"),
                    FechaPago = reader.GetDateTime("FechaPago"),
                    Importe = reader.GetDecimal("Importe"),
                    Anulado = reader.GetBoolean("Anulado"),
                    CreadoPor = reader.IsDBNull(reader.GetOrdinal("CreadoPor")) ? null : reader.GetInt32("CreadoPor"),
                    AnuladoPor = reader.IsDBNull(reader.GetOrdinal("AnuladoPor")) ? null : reader.GetInt32("AnuladoPor"),
                    Reserva = new Reserva
                    {
                        IdReserva = reader.GetInt32("IdReserva"),
                        Inquilino = new Inquilino
                        {
                            IdInquilino = reader.GetInt32("ID_Inquilino"),
                            Nombre = reader.GetString("InquilinoNombre"),
                            Apellido = reader.GetString("InquilinoApellido")
                        }
                    }
                });
            }

            return lista;
        }
    }
}