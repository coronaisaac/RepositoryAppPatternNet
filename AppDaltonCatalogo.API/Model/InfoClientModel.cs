namespace AppDaltonCatalogo.API.NewFolder
{
    public record InfoClientModel
    {
        public int? IsDataClient { get; set; }
        public Client Client { get; set; }
    }
    public record Client
    {
        public int? ID { get; set; }
        public string Nombre { get; set; }
        public string ApMaterno { get; set; }
        public string ApPaterno { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public int? TipoCliente { get; set; }
    }

}
