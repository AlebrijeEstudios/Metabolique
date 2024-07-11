namespace AppVidaSana.Models.Dtos.Ejercicio_Dtos
{
    public class ListaEjerciciosDto
    {
        public Guid idejercicio { get; set; } = Guid.NewGuid();

        public string tipo { get; set; } = null!;

        public string intensidad { get; set; } = null!;

        public int tiempo { get; set; }
    }
}
