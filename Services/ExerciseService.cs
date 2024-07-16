using AppVidaSana.Data;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Services.IServices;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Services
{
    public class ExerciseService : IExercise
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public ExerciseService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public List<ExerciseListDto> GetExercises(Guid id, DateOnly date)
        {
            var exercise = _bd.Exercises
            .Where(e => e.accountID == id && e.dateExercise == date)
            .ToList();

            if(exercise.Count == 0)
            {
                throw new ExerciseNotFoundException();
            }

            var exercises = _mapper.Map<List<ExerciseListDto>>(exercise);

            return exercises;
        }

        public string UpdateExercises(Guid idexercise, ExerciseListDto exercise)
        {
            var ex = _bd.Exercises.Find(idexercise);

            if (ex == null)
            {
                throw new ExerciseNotFoundException();
            }

            ex.typeExercise = exercise.typeExercise;
            ex.intensityExercise = exercise.intensityExercise;
            ex.timeSpent = exercise.timeSpent;


            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(ex, null, null);

            if (!Validator.TryValidateObject(ex, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }

            _bd.Exercises.Update(ex);
            Save();
            return "Actualización completada";

        }

        public string AddExercises(AddExerciseDto exercise)
        {
            var user = _bd.Accounts.Find(exercise.accountID);

            if (user == null)
            {
                return "No se guardaron los datos, intentelo de nuevo";
            }

            Exercise ex = new Exercise
            {
                accountID = exercise.accountID,
                dateExercise = exercise.dateExercise,
                typeExercise = exercise.typeExercise,
                intensityExercise = exercise.intensityExercise,
                timeSpent = exercise.timeSpent,
                account = null
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(ex, null, null);

            if (!Validator.TryValidateObject(ex, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }

            _bd.Exercises.Add(ex);
            Save();
            return "Los datos han sido guardados correctamente";
        }

        public string DeleteExercise(Guid idexercise)
        {
            var ex = _bd.Exercises.Find(idexercise);
            if (ex == null)
            {
                throw new ExerciseNotFoundException();
            }

            _bd.Exercises.Remove(ex);
            Save();
            return "Se ha eliminado correctamente.";
        }

        public bool Save()
        {
            try
            {
                return _bd.SaveChanges() >= 0;
            }
            catch (Exception)
            {
                return false;

            }
        } 
    }
}
