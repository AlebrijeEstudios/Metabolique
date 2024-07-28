using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Graphics;
using AppVidaSana.Models.Dtos.Graphics_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IExercise
    {
        List<ExerciseListDto> GetExercises(Guid id, DateOnly date);

        List<GExerciseDto> ValuesGraphicExercises(Guid id, DateOnly date);

        string AddExercises(AddExerciseDto exercise);

        string UpdateExercises(ExerciseListDto exercise);

        string DeleteExercise(Guid idexercise);

        bool Save();
    }
}
