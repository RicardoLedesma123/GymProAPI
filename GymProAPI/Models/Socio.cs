namespace GymProAPI.Models
{
    public class Socio
    {
        public int SocioID { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public bool ActivoInactivo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaBaja { get; set; }
        public DateTime? FechaPago { get; set; }
        public bool AlCorriente { get; set; }
        public string? FotoUrl { get; set; }
        public string Email { get; set; } = string.Empty;

        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    }
}
