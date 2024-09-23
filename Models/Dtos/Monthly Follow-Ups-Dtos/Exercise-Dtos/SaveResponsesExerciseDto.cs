using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos
{
    public class SaveResponsesExerciseDto
    {
        [JsonRequired] public Guid accountID { get; set; } 
         
        [JsonRequired] public int month { get; set; } 

        [JsonRequired] public int year { get; set; }

        [JsonRequired] public int question1 { get; set; }

        [JsonRequired] public int question2 { get; set; }

        [JsonRequired] public int question3 { get; set; }

        [JsonRequired] public int question4 { get; set; }

        [JsonRequired] public int question5 { get; set; }

        [JsonRequired] public int question6 { get; set; }

        [JsonRequired] public int question7 { get; set; }
    }
}
