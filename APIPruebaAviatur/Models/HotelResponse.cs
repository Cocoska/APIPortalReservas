namespace APIPruebaAviatur.Models
{
    public class HotelResponse
    {
        public String Nombre { get; set; }
        public String Descripcion { get; set; }
        public decimal Precio { get; set; }
        public double Rating { get; set; }
        public int proveedorHotelId { get; set; }
    }
}
