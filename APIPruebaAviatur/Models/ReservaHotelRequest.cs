namespace APIPruebaAviatur.Models
{
    public class ReservaHotelRequest
    {
        public string EmailCliente { get; set; }
        public string NombreCliente { get; set; }
        public string NombreProveedor { get; set; }
        public string NombreHotel { get; set; }
        public string CiudadHotel { get; set; }
        public DateTime FechaCheckIn { get; set; }
        public DateTime FechaCheckOut { get; set; }
        public decimal PrecioTotal { get; set; }
        public string Moneda { get; set; }
        public string EstadoReserva { get; set; }
        public long? BusquedaId { get; set; }
        public string? CodigoReservaProveedor { get; set; }
    }
}
