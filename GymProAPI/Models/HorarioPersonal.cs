using System.ComponentModel.DataAnnotations;

namespace GymProAPI.Models
{
    public class HorarioPersonal
    {
        [Key]
        public int HorarioID { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
        public string ColorHex { get; set; } = "#198754";
        public DateTime? Fecha { get; set; }
    }
}