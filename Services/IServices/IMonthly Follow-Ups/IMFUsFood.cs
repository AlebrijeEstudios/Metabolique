using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos;

namespace AppVidaSana.Services.IServices.IMonthly_Follow_Ups
{
    public interface IMFUsFood
    {
        Task<ResultsMFUsFoodDto?> SaveAnswersAsync(AnswersMFUsFoodDto values, CancellationToken cancellationToken);
         
        Task<ResultsMFUsFoodDto?> RetrieveAnswersAsync(Guid accountID, int month, int year, CancellationToken cancellationToken);

        ResultsMFUsFoodDto UpdateAnswers(UpdateAnswersMFUsFoodDto values);

        bool Save();

    }
}
