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
                (IdReserva, Concepto, FechaPago, Importe, Anulado)
                VALUES
                (@IdReserva, @Concepto, @FechaPago, @Importe, 0);

                SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@IdReserva", pago.IdReserva);
            command.Parameters.AddWithValue("@Concepto", pago.Concepto);
            command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
            command.Parameters.AddWithValue("@Importe", pago.Importe);

            connection.Open();

            res = Convert.ToInt32(command.ExecuteScalar());

            return res;
        }

        public int Modificacion(Pago pago)
        {
            int res = -1;

            using var connection = new MySqlConnection(connectionString);

            // La narrativa dice que al editar
            // solamente puede modificarse el concepto.
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

        public int Anular(int idPago)
        {
            int res = -1;

            using var connection = new MySqlConnection(connectionString);

            // No se elimina físicamente:
            // solamente se cambia su estado.
            string sql = @"
                UPDATE pago
                SET Anulado = 1
                WHERE IdPago = @IdPago;";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@IdPago", idPago);

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
                    IdPago,
                    IdReserva,
                    Concepto,
                    FechaPago,
                    Importe,
                    Anulado
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
                    Anulado = reader.GetBoolean("Anulado")
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
                    IdPago,
                    IdReserva,
                    Concepto,
                    FechaPago,
                    Importe,
                    Anulado
                FROM pago
                WHERE IdPago = @IdPago;";

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
                    Anulado = reader.GetBoolean("Anulado")
                };
            }

            return pago;
        }

        public IList<Pago> ObtenerPorReserva(int idReserva)
        {
            var lista = new List<Pago>();

            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                SELECT
                    IdPago,
                    IdReserva,
                    Concepto,
                    FechaPago,
                    Importe,
                    Anulado
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
                    Anulado = reader.GetBoolean("Anulado")
                });
            }

            return lista;
        }
    }
}