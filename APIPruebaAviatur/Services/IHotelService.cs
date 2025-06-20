using APIPruebaAviatur.Models;

namespace APIPruebaAviatur.Services
{
    public interface IHotelService
    {
        Task<List<HotelResponse>> BuscarHotelesAsync(string ciudad, DateTime checkIn, DateTime checkOut, int huespedes);
    }
}
