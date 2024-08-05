using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IExercise
    {
        List<ExerciseListDto> GetExercises(Guid id, DateOnly date);

        List<GraphicsValuesExerciseDto> ValuesGraphicExercises(Guid id, DateOnly date);

        List<ExerciseListDto> AddExercises(AddExerciseDto exercise);

        List<ExerciseListDto> UpdateExercises(ExerciseListDto exercise);

        List<ExerciseListDto> DeleteExercise(Guid idexercise);

        bool Save();
    }
}
