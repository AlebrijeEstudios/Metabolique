using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos;

namespace AppVidaSana.ProducesResponseType.Food.MFUsFood
{
    public class MFUsFoodResponse
    {
        public string message { get; set; } = "Ok.";

        public ResultsMFUsFoodDto? mfus { get; set; } 
    }
}
