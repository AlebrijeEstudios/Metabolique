using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos;

namespace AppVidaSana.Services.IServices.IMonthly_Follow_Ups
{
    public interface IMFUsFood
    {
        ResultsMFUsFoodDto SaveAnswers(AnswersMFUsFoodDto values);

        ResultsMFUsFoodDto RetrieveAnswers(Guid accountID, int month, int year);

        ResultsMFUsFoodDto UpdateAnswers(UpdateAnswersMFUsFoodDto values);

        bool Save();

    }
}
