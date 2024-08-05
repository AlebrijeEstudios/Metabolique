using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;
using AppVidaSana.Models.Exercises;
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

            List<ExerciseListDto> exercises;

            if (exercise.Count == 0)
            {
                exercises = _mapper.Map<List<ExerciseListDto>>(exercise);
            }

            exercises = _mapper.Map<List<ExerciseListDto>>(exercise);

            return exercises;
        }

        public List<GraphicsValuesExerciseDto> ValuesGraphicExercises(Guid id, DateOnly date)
        {
            DateOnly dateFinal = date.AddDays(-6);

            var events = _bd.MinutesConsumed
                .Where(e => e.dateExercise >= dateFinal && e.dateExercise <= date && e.accountID == id)
                .ToList();

            List<GraphicsValuesExerciseDto> gExercises;

            if (events.Count == 0)
            {
                gExercises = _mapper.Map<List<GraphicsValuesExerciseDto>>(events);
            }

            gExercises = _mapper.Map<List<GraphicsValuesExerciseDto>>(events);

            gExercises = gExercises.OrderBy(x => x.dateExercise).ToList();

            return gExercises;
        }

        public List<ExerciseListDto> AddExercises(AddExerciseDto exercise)
        {
            var exerciseExisting = _bd.Exercises.Count(e => e.dateExercise == exercise.dateExercise && e.typeExercise == exercise.typeExercise &&
                                e.intensityExercise == exercise.intensityExercise && e.timeSpent == exercise.timeSpent);

            if(exerciseExisting > 0)
            {
                throw new RepeatRegistrationException();
            }

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

            List<ExerciseListDto> exercises = GetExercises(exercise.accountID, exercise.dateExercise);

            return exercises;
        }

        public List<ExerciseListDto> UpdateExercises(ExerciseListDto exercise)
        {
            var ex = _bd.Exercises.Find(exercise.exerciseID);

            if (ex == null)
            {
                throw new ExerciseNotFoundException();
            }

            if(ex.timeSpent < exercise.timeSpent || ex.timeSpent > exercise.timeSpent)
            {
                var previousTotal = _bd.MinutesConsumed.FirstOrDefault(e => e.dateExercise == ex.dateExercise);

                int currentTotal = previousTotal.totalTimeSpent - ex.timeSpent;
                int newTotal = currentTotal + exercise.timeSpent;

                previousTotal.totalTimeSpent = newTotal;

                _bd.MinutesConsumed.Update(previousTotal);

                if (!Save())
                {
                    throw new UnstoredValuesException();
                }
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

            List<ExerciseListDto> exercises = GetExercises(exercise.accountID, exercise.dateExercise);

            return exercises;
        }

        public List<ExerciseListDto> DeleteExercise(Guid idexercise)
        {
            var ex = _bd.Exercises.Find(idexercise);

            if (ex == null)
            {
                throw new ExerciseNotFoundException();
            }

            Guid id = ex.accountID;
            DateOnly date = ex.dateExercise;

            var exerciseExisting = _bd.Exercises.Count(e => e.dateExercise == ex.dateExercise);
            var previousTotal = _bd.MinutesConsumed.FirstOrDefault(e => e.dateExercise == ex.dateExercise);

            if (exerciseExisting >= 2)
            {
                if (previousTotal != null)
                {
                    int currentTotal = previousTotal.totalTimeSpent - ex.timeSpent;
                    int newTotal = currentTotal;

                    previousTotal.totalTimeSpent = newTotal;

                    _bd.MinutesConsumed.Update(previousTotal);

                    if (!Save())
                    {
                        throw new UnstoredValuesException();
                    }
                }
            }

            if (exerciseExisting == 1)
            {
                if (previousTotal != null)
                {
                    _bd.MinutesConsumed.Remove(previousTotal);

                    if (!Save())
                    {
                        throw new UnstoredValuesException();
                    }
                }
            }

            _bd.Exercises.Remove(ex);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }


            List<ExerciseListDto> exercises = GetExercises(id, date);

            return exercises;

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

        private void totalTimeSpentforDay(Guid id, DateOnly dateInitial, int timeSpent)
        {
            var infoGraphics = _bd.MinutesConsumed.FirstOrDefault(c => c.accountID == id && c.dateExercise == dateInitial);

            if(infoGraphics != null)
            {            
                var value = infoGraphics.totalTimeSpent;

                infoGraphics.totalTimeSpent = value + timeSpent;

                _bd.MinutesConsumed.Update(infoGraphics);

                if (!Save())
                {
                    throw new UnstoredValuesException();
                }
            }
            else
            {
                minutesConsumed dates = new minutesConsumed
                {
                    accountID = id,
                    dateExercise = dateInitial,
                    totalTimeSpent = timeSpent
                };

                _bd.MinutesConsumed.Add(dates);

                if (!Save())
                {
                    throw new UnstoredValuesException();
                }
            }
        } 
    }
}
