using APIPruebaAviatur.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace APIPruebaAviatur.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IConfiguration _configuration;

        public ReservaService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ReservaHotelResponse> RealizarReservaAsync(ReservaHotelRequest request)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                return new ReservaHotelResponse {Exito = false, Mensaje = "La cadena de conexion a la base de datos no esta configurada" };
            }

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    
                    using (var command = new SqlCommand("sp_InsertarReservaSabre", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@UsuarioEmail", request.EmailCliente);
                        command.Parameters.AddWithValue("@ProveedorNombre", request.NombreProveedor);
                        command.Parameters.AddWithValue("@BusquedaId", request.BusquedaId.HasValue ? (object)request.BusquedaId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@CodigoReservaProveedor", request.CodigoReservaProveedor);
                        command.Parameters.AddWithValue("@NombreHotel", request.NombreHotel);
                        command.Parameters.AddWithValue("@CiudadHotel", request.CiudadHotel);
                        command.Parameters.AddWithValue("@FechaCheckIn", request.FechaCheckIn);
                        command.Parameters.AddWithValue("@FechaCheckOut", request.FechaCheckOut);
                        command.Parameters.AddWithValue("@Preciototal", request.PrecioTotal);
                        command.Parameters.AddWithValue("@Moneda", request.Moneda);
                        command.Parameters.AddWithValue("@EstadoReserva", request.EstadoReserva);


                        var reservaIdparam = new SqlParameter("@ReservaId_OUT", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                        command.Parameters.Add(reservaIdparam);
                        var resultadoMensajeParam = new SqlParameter("@ResultadoMensaje", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
                        command.Parameters.Add(resultadoMensajeParam);

                        await command.ExecuteNonQueryAsync();

                        long? reservaId = reservaIdparam.Value != DBNull.Value ? (long?)reservaIdparam.Value : null;
                        string mensaje = resultadoMensajeParam.Value?.ToString();

                        return new ReservaHotelResponse
                        {
                            Exito = reservaId.HasValue,
                            ReservaId = reservaId,
                            Mensaje = mensaje
                        };
                    }
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"Error de SQL al procesar la reserva: {sqlEx.Message}");
                    return new ReservaHotelResponse { Exito = false, Mensaje = $"Errorn de base de dator: {sqlEx.Message}" };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inesperado en RealizarReservaAsync: {ex.Message}");
                    return new ReservaHotelResponse { Exito = false, Mensaje = $"Error interno en el servidor: {ex.Message}"};
                }
            }
        }

        public async Task<List<ReservaReporteResponse>> ObtenerReservasAsync(
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            int? proveedorID = null,
            int? empresaID = null,
            int? usuarioID = null,
            string? ciudadHotel = null,
            string? estadoReserva = null)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            Debug.WriteLine("[API-Service] Iniciando ObtenerReservasAsync.");
            var reservas = new List<ReservaReporteResponse>();

            if (String.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine("[API-Service] Error: La cadena de conexión no está configurada para ObtenerReservasAsync.");
                return reservas;
            }

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    Debug.WriteLine("[API-Service] Conexión a DB abierta para reporte.");

                    using (var command = new SqlCommand("sp_ObtenerReporteReservas", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@FechaDesde", fechaDesde.HasValue ? (object)fechaDesde.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@FechaHasta", fechaHasta.HasValue ? (object)fechaHasta.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@ProveedorID", proveedorID.HasValue ? (object)proveedorID.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@EmpresaID", empresaID.HasValue ? (object)empresaID.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@UsuarioID", usuarioID.HasValue ? (object)usuarioID.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@CiudadHotel", string.IsNullOrEmpty(ciudadHotel) ? DBNull.Value : (object)ciudadHotel);
                        command.Parameters.AddWithValue("@EstadoReserva", string.IsNullOrEmpty(estadoReserva) ? DBNull.Value : (object)estadoReserva);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reservas.Add(new ReservaReporteResponse
                                {
                                    ReservsaId = reader.GetInt64(reader.GetOrdinal("ReservaID")),
                                    CodigoReservaProveedor = reader.GetString(reader.GetOrdinal("CodigoReservaProveedor")),
                                    NombreHotel = reader.GetString(reader.GetOrdinal("NombreHotel")),
                                    CiudadHotel = reader.GetString(reader.GetOrdinal("CiudadHotel")),
                                    FechaCheckIn = reader.GetDateTime(reader.GetOrdinal("FechaCheckIn")),
                                    FechaCheckOut = reader.GetDateTime(reader.GetOrdinal("FechaCheckOut")),
                                    PrecioTotal = reader.GetDecimal(reader.GetOrdinal("PrecioTotal")),
                                    Moneda = reader.GetString(reader.GetOrdinal("Moneda")),
                                    EstadoReserva = reader.GetString(reader.GetOrdinal("EstadoReserva")),
                                    FechaCreacionReserva = reader.GetDateTime(reader.GetOrdinal("FechaCreacionReserva")),
                                    NombreCliente = reader.IsDBNull(reader.GetOrdinal("NombreUsuario")) ? null : reader.GetString(reader.GetOrdinal("NombreUsuario")),
                                    NombreProveedor = reader.IsDBNull(reader.GetOrdinal("NombreProveedor")) ? null : reader.GetString(reader.GetOrdinal("NombreProveedor")),
                                    EmailUsuario = reader.IsDBNull(reader.GetOrdinal("EmailUsuario")) ? null : reader.GetString(reader.GetOrdinal("EmailUsuario")),
                                    NombreEmpresa = reader.IsDBNull(reader.GetOrdinal("NombreEmpresa")) ? null : reader.GetString(reader.GetOrdinal("NombreEmpresa"))
                                });
                            }
                        }
                    }
                    Debug.WriteLine($"[API-Service] Se encontraron {reservas.Count} reservas para el reporte.");
                }
                catch (SqlException sqlEx)
                {
                    Debug.WriteLine($"[API-Service] Error de SQL al obtener reservas para el reporte: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[API-Service] Error inesperado en ObtenerReservasAsync: {ex.Message}");
                }
            }
            return reservas;
        }
    }
}
