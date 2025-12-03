namespace GymProAPI.Models
{
    public class Visita
    {
        public int VisitaID { get; set; }
        public int NumeroPersonas { get; set; }
        public decimal TotalRecaudado { get; set; }
        public DateTime FechaVisita { get; set; }
    }
}