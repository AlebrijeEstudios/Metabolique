using AppVidaSana.Models.Dtos.Habits_Dtos;

namespace AppVidaSana.Services.IServices.IHabits
{
    public interface IHabitsGeneral
    {
        ReturnInfoHabitsDto GetInfoGeneralHabits(Guid idAccount, DateOnly date);
    }
}
