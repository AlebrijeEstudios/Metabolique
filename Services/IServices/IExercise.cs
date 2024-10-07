using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Exercise_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IExercise
    {
        List<ExerciseListDto> GetExercises(Guid id, DateOnly date);

        ExerciseAndValuesGraphicDto ExercisesAndValuesGraphic(Guid id, DateOnly date);

        ExerciseListDto AddExercises(AddExerciseDto exercise);

        ExerciseListDto UpdateExercises(ExerciseListDto exercise);

        string DeleteExercise(Guid idexercise);

        bool Save();
    }
}
