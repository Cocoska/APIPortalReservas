using APIPruebaAviatur.Models;
using APIPruebaAviatur.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace APIPruebaAviatur.Controllers
{
    [ApiController]
    [Route("api/Hoteles")]
    public class HotelesController : ControllerBase
    {
        private readonly IReservaService _reservaService;
        private readonly IHotelService _hotelService;

        public HotelesController (IReservaService reservaService, IHotelService hotelService)
        {
            _reservaService = reservaService;
            _hotelService = hotelService;
        }

        [HttpPost("buscar")]
        public async Task<IActionResult> BuscarHoteles([FromBody] BusquedaHotelRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hoteles = await _hotelService.BuscarHotelesAsync(
                request.Ciudad,
                request.CheckIn,
                request.CheckOut,
                request.Huespedes
            );

            if (hoteles == null || !hoteles.Any())
            {
                return NotFound("No se encontraron hoteles con los criterios especificados");
            }

            return Ok(hoteles);
        }

        [HttpPost("reserva")]
        public async Task<IActionResult> ReservarHotel([FromBody] ReservaHotelRequest request)
        {
            request.CodigoReservaProveedor = $"AVIATUR--{DateTimeOffset.Now.ToUnixTimeMilliseconds()}--{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)
                                            .ToList();
                var mensajeError = string.Join(" | ", errorMessages);
                if (string.IsNullOrEmpty(mensajeError)) { mensajeError = "Datos de reserva inválidos."; }

                Debug.WriteLine($"[API-Controller] Errores de ModelState: {mensajeError}");
                return BadRequest(new ReservaHotelResponse
                {
                    Exito = false,
                    Mensaje = $"Error de validación: {mensajeError}"
                });
            }
                        
            var response = await _reservaService.RealizarReservaAsync(request);

            if (!response.Exito)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("reporteReservas")]
        public async Task<IActionResult> ObtenerReporteReservas(
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta,
            [FromQuery] int? proveedorID,
            [FromQuery] int? empresaID,
            [FromQuery] int? usuarioID,
            [FromQuery] string? ciudadHotel,
            [FromQuery] string? estadoReserva)
        {
            Debug.WriteLine("[API-Controller] Solicitud recibida para ObtenerReporteReservas.");

            try
            {
                var reservas = await _reservaService.ObtenerReservasAsync(
                    fechaDesde,
                    fechaHasta,
                    proveedorID,
                    empresaID,
                    usuarioID,
                    ciudadHotel,
                    estadoReserva
                );

                if (reservas == null || !reservas.Any())
                {
                    Debug.WriteLine("[API-Controller] No se encontraron reservas para el reporte.");
                    return NotFound("No se encontraron reservas.");
                }
                if (!reservas.Any())
                {
                    Debug.WriteLine("[API-Controller] No se encontraron reservas para el reporte con los filtros especificados. Devolviendo lista vacía.");
                    return Ok(new List<ReservaReporteResponse>());
                }
                Debug.WriteLine($"[API-Controller] Devolviendo {reservas.Count} reservas para el reporte.");
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API-Controller] Error en ObtenerReporteReservas: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor al obtener el reporte: {ex.Message}");
            }
        }
    }
}
