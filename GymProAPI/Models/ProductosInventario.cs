using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GymProAPI.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Presentacion { get; set; }
        public string Categoria { get; set; }
        public int StockActual { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? Caducidad { get; set; }
        public string? Proveedor { get; set; }
        public string Estado { get; set; }

        // Relación con movimientos
        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
    }

    public class MovimientoInventario
    {
        [Key]
        public int MovimientoID { get; set; }
        public int ProductoID { get; set; }
        public string TipoMovimiento { get; set; } // Entrada / Salida
        public int Cantidad { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Observaciones { get; set; }
        public string Usuario { get; set; }

        // Relación con producto
        [JsonIgnore]
        public Producto? Producto { get; set; }
    }

    public class GananciaProductos
    {
        public int Mes { get; set; }
        public int Año { get; set; }
        public decimal GananciaNeta { get; set; }
    }
}