using APIPruebaAviatur.Models;

namespace APIPruebaAviatur.Services
{
    public interface IReservaService
    {
        Task<ReservaHotelResponse> RealizarReservaAsync(ReservaHotelRequest request);
        Task<List<ReservaReporteResponse>> ObtenerReservasAsync(

            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            int? proveedorID = null,
            int? empresaID = null,
            int? usuarioID = null,
            string? ciudadHotel = null,
            string? estadoReserva = null
        );
    }
}
