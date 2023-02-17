namespace AppDaltonCatalogo.API.Model
{
    public class ShoppingClientModel
    {
        public int CarritoID { get; set; }
        public double Cantidad { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen { get; set; }
        public string? Sucursal { get; set; }
        public string? Region { get; set; }
        public DateTime Fecha { get; set; }
    }
}
