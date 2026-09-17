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

        public IList<Pago> ObtenerLista()
        {
            var lista = new List<Pago>();

            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                SELECT
                    IdPago, IdReserva, Concepto, FechaPago, Importe, Anulado, CreadoPor, AnuladoPor
                FROM pago
                ORDER BY FechaPago DESC;";

            using var command = new MySqlCommand(sql, connection);
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
    }
}