namespace APIPruebaAviatur.Models
{
    public class BusquedaHotelRequest
    {
        public String Ciudad { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Huespedes { get; set; }
    }
}
