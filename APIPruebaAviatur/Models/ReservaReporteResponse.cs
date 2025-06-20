namespace APIPruebaAviatur.Models
{
    public class ReservaReporteResponse
    {
        public long ReservsaId { get; set; }
        public string CodigoReservaProveedor { get; set; } = String.Empty;
        public string NombreHotel { get; set; } = String.Empty;
        public string CiudadHotel { get; set; } = String.Empty;
        public DateTime FechaCheckIn { get; set; }
        public DateTime FechaCheckOut { get; set; }
        public decimal PrecioTotal { get; set; }
        public string Moneda { get; set; } = String.Empty;
        public string EstadoReserva { get; set; } = String.Empty;
        public DateTime FechaCreacionReserva { get; set; }
        public string? EmailCliente { get; set; }
        public string NombreCliente { get; set; }
        public string? NombreProveedor { get; set; }

        public string? EmailUsuario { get; set; }

        public string? NombreEmpresa { get; set; }
    }
}
