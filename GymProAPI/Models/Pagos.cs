using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GymProAPI.Models
{
    public class Pago
    {
        public int PagoID { get; set; }

        // Relación con Socio
        public int SocioID { get; set; }

        // Datos del pago
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } // Efectivo, Tarjeta, Transferencia
        public bool AlCorriente { get; set; }

        // Extras
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [JsonIgnore]
        [ValidateNever]
        public Socio Socio { get; set; }

    }
}
