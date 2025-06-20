using APIPruebaAviatur.Models;
using Microsoft.Data.SqlClient;

namespace APIPruebaAviatur.Services
{
    public class HotelService : IHotelService
    {
        private readonly DummyJsonClient _dummyJsonClient;
        private readonly string _connectionString;

        public HotelService(DummyJsonClient dummyJsonClient, IConfiguration configuration)
        {
            _dummyJsonClient = dummyJsonClient;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<HotelResponse>> BuscarHotelesAsync(string ciudad, DateTime checkIn, DateTime checkOut, int huespedes)
        {
            var dummyResponse = await _dummyJsonClient.SearchProductsAsync("Hotel");
            List<HotelResponse> hotelesEncontrados = new List<HotelResponse>();

            if (dummyResponse != null && dummyResponse.products != null)
            {
                hotelesEncontrados = dummyResponse.products
                .Where(p => p.category?.ToLower() == "hotel" || p.title.ToLower().Contains("hotel") || p.description.ToLower().Contains("hotel"))
                .Select(p => new HotelResponse
                {
                    Nombre = p.title,
                    Descripcion = p.description,
                    Precio = p.price,
                    Rating = p.rating,
                    proveedorHotelId = p.id
                }).ToList();
            }
            long busquedaId = 0;
            string mensageBd = "";
            bool exitoBd = false;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_InsertarBusquedaHotel", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Ciudad", ciudad);
                    command.Parameters.AddWithValue("@CheckIn", checkIn);
                    command.Parameters.AddWithValue("@CheckOut", checkOut);
                    command.Parameters.AddWithValue("@Huespedes", huespedes);

                    SqlParameter busquedaIdParam = new SqlParameter("@BusquedaID", System.Data.SqlDbType.BigInt);
                    busquedaIdParam.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(busquedaIdParam);

                    SqlParameter mensajeParam = new SqlParameter("@Mensaje", System.Data.SqlDbType.NVarChar, 255);
                    mensajeParam.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(mensajeParam);

                    SqlParameter exitoParam = new SqlParameter("@Exito", System.Data.SqlDbType.Bit);
                    exitoParam.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(exitoParam);

                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();

                        busquedaId = (long)busquedaIdParam.Value;
                        mensageBd = mensajeParam.Value.ToString();
                        exitoBd = (bool)exitoParam.Value;

                        Console.WriteLine($"DEBUG: Búsqueda registrada en DB. ID: {busquedaId}, Mensaje: {mensageBd}");

                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"ERROR: Fallo al registrar la búsqueda en la DB: {ex.Message}");
                    }
                }
            }
            return hotelesEncontrados;
        }
    }
}
