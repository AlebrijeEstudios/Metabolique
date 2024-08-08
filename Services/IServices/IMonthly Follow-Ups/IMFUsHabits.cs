using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;

namespace AppVidaSana.Services.IServices.IMonthly_Follow_Ups
{
    public interface IMFUsHabits
    {
        SaveResultsHabitsDto SaveAnswers(SaveResponsesHabitsDto res);

        public string SaveResults(SaveResultsHabitsDto res);

        RetrieveResponsesHabitsDto RetrieveAnswers(Guid id, int month, int year);

        bool Save();

    }
}
