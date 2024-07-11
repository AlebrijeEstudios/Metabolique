namespace AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos
{
    public class GuardarRespuestasDto
    {
        public Guid id { get; set; }

        public string mes { get; set; } = null!;

        public int año { get; set; }

        public int pregunta1 { get; set; }

        public int pregunta2 { get; set; }

        public int pregunta3 { get; set; }

        public int pregunta4 { get; set; }

        public int pregunta5 { get; set; }

        public int pregunta6 { get; set; }

        public int pregunta7 { get; set; }
    }
}
