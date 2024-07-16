using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IExercise
    {
        List<ExerciseListDto> GetExercises(Guid id, DateOnly date);

        string AddExercises(AddExerciseDto exercise);

        string UpdateExercises(Guid idexercise, ExerciseListDto exercise);

        string DeleteExercise(Guid idexercise);

        bool Save();
    }
}
