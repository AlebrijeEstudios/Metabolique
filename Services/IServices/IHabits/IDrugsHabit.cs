using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;
using Microsoft.AspNetCore.JsonPatch;

namespace AppVidaSana.Services.IServices.IHabits.IHabits
{
    public interface IDrugsHabit
    {
        Task<DrugsHabitInfoDto> AddDrugsConsumedAsync(DrugsHabitDto values, CancellationToken cancellationToken);

        Task<DrugsHabitInfoDto> UpdateDrugsConsumedAsync(Guid drugsHabitID, JsonPatchDocument values, CancellationToken cancellationToken);

        bool Save();
    }
}
