using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;

namespace AppVidaSana.Services.IServices.IMonthly_Follow_Ups
{
    public interface IMFUsHabits
    {
        RetrieveResponsesHabitsDto SaveAnswers(SaveResponsesHabitsDto values);

        RetrieveResponsesHabitsDto RetrieveAnswers(Guid id, int month, int year);

        RetrieveResponsesHabitsDto UpdateAnswers(UpdateResponsesHabitsDto values);

        bool Save();

    }
}
