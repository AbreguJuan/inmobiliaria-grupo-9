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
                    (ID_Inquilino, ID_Inmueble, Desde, Hasta, ID_Pago)
                    VALUES (@idInquilino, @idInmueble, @desde, @hasta, @idPago);
                    SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
                    command.Parameters.AddWithValue("@desde", r.Desde);
                    command.Parameters.AddWithValue("@hasta", r.Hasta);
                    command.Parameters.AddWithValue("@idPago", (object?)r.IdPago ?? DBNull.Value);
                    connection.Open();
                    res = System.Convert.ToInt32(command.ExecuteScalar());
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
                    ID_Inquilino=@idInquilino, ID_Inmueble=@idInmueble, Desde=@desde, Hasta=@hasta, ID_Pago=@idPago
                    WHERE ID_Reserva=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
                    command.Parameters.AddWithValue("@desde", r.Desde);
                    command.Parameters.AddWithValue("@hasta", r.Hasta);
                    command.Parameters.AddWithValue("@idPago", (object?)r.IdPago ?? DBNull.Value);
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
                string sql = @$"SELECT ID_Reserva AS IdReserva, ID_Inquilino AS IdInquilino,
                        ID_Inmueble AS IdInmueble, Desde, Hasta, ID_Pago AS IdPago
                    FROM reserva
                    ORDER BY ID_Reserva
                    LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";
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
                            IdPago = reader.IsDBNull(reader.GetOrdinal("IdPago")) ? null : reader.GetInt32("IdPago"),
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
                    res = System.Convert.ToInt32(command.ExecuteScalar());
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
                string sql = @"SELECT ID_Reserva AS IdReserva, ID_Inquilino AS IdInquilino,
                        ID_Inmueble AS IdInmueble, Desde, Hasta, ID_Pago AS IdPago
                    FROM reserva WHERE ID_Reserva=@id";
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
                            IdPago = reader.IsDBNull(reader.GetOrdinal("IdPago")) ? null : reader.GetInt32("IdPago"),
                        };
                    }
                    connection.Close();
                }
            }
            return r;
        }
    }
}