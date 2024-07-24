using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;
using AppVidaSana.Models.Graphics;
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

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Actualización completada.";

        }

        public string AddExercises(AddExerciseDto exercise)
        {
            var user = _bd.Accounts.Find(exercise.accountID);

            if (user == null)
            {
                throw new UserNotFoundException();
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

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            totalTimeSpentforDay(exercise.accountID, exercise.dateExercise, exercise.timeSpent);
            
            return "Los datos han sido guardados correctamente.";
        }

        public string DeleteExercise(Guid idexercise)
        {
            var ex = _bd.Exercises.Find(idexercise);
            if (ex == null)
            {
                throw new ExerciseNotFoundException();
            }

            _bd.Exercises.Remove(ex);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Se ha eliminado correctamente.";
        }

        public List<GExerciseDto> ValuesGraphicExercises(Guid id, DateOnly date)
        {
            DateOnly dateFinal = date.AddDays(-6);

            var events = _bd.graphicsExercise
                .Where(e => e.dateExercise >= dateFinal && e.dateExercise <= date && e.accountID == id)
                .ToList();

            if (events.Count == 0)
            {
                throw new ExerciseNotFoundException();
            }

            var gExercises = _mapper.Map<List<GExerciseDto>>(events);

            return gExercises;
        }


        public void totalTimeSpentforDay(Guid id, DateOnly dateInitial, int timeSpent)
        {
            var infoGraphics = _bd.graphicsExercise.FirstOrDefault(c => c.accountID == id && c.dateExercise == dateInitial);

            if(infoGraphics != null)
            {            
                var value = infoGraphics.totalTimeSpent;

                infoGraphics.totalTimeSpent = value + timeSpent;

                _bd.graphicsExercise.Update(infoGraphics);
                Save();
            }
            else
            {
                GExercise dates = new GExercise
                {
                    accountID = id,
                    dateExercise = dateInitial,
                    totalTimeSpent = timeSpent
                };

                _bd.graphicsExercise.Add(dates);
                Save();
            }
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
