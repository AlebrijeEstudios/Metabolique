using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Exercise_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;
using AppVidaSana.Models.Exercises;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

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
            var exercise = _bd.Exercises.Where(e => e.accountID == id && e.dateExercise == date).ToList();

            List<ExerciseListDto> exercises;

            if (exercise.Count == 0)
            {
                exercises = _mapper.Map<List<ExerciseListDto>>(exercise);
            }

            exercises = _mapper.Map<List<ExerciseListDto>>(exercise);

            return exercises;
        }

        public ExerciseAndValuesGraphicDto ExercisesAndValuesGraphic(Guid id, DateOnly date)
        {
            ExerciseAndValuesGraphicDto info;

            List<ExerciseListDto> exercises = GetExercises(id, date);

            DateOnly dateFinal = date.AddDays(-6);

            var dates = GetDatesInRange(dateFinal, date);

            List<GraphicValuesExerciseDto> graphicValues = new List<GraphicValuesExerciseDto>();

            foreach (var item in dates)
            {
                var activeMinutes = _bd.ActiveMinutes.FirstOrDefault(e => e.dateExercise == item && e.accountID == id);

                if (activeMinutes is not null)
                {
                    GraphicValuesExerciseDto value = new GraphicValuesExerciseDto
                    {
                        date = item,
                        value = activeMinutes.totalTimeSpent
                    };

                    graphicValues.Add(value);
                }
                else
                {
                    GraphicValuesExerciseDto value = new GraphicValuesExerciseDto
                    {
                        date = item,
                        value = 0
                    };

                    graphicValues.Add(value);
                }
            }

            graphicValues = graphicValues.OrderBy(x => x.date).ToList();

            CultureInfo ci = new CultureInfo("es-ES");

            var monthExist = _bd.Months.FirstOrDefault(e => e.month == date.ToString("MMMM", ci)
                                                       && e.year == Convert.ToInt32(date.ToString("yyyy")));

            if (monthExist == null)
            {
                info = new ExerciseAndValuesGraphicDto
                {
                    exercises = exercises,
                    activeMinutes = graphicValues,
                    mfuStatus = false
                };

                return info;
            }

            var mfuExist = _bd.MFUsExercise.Any(e => e.accountID == id
                                                && e.monthID == monthExist.monthID);

            if (!mfuExist)
            {
                info = new ExerciseAndValuesGraphicDto
                {
                    exercises = exercises,
                    activeMinutes = graphicValues,
                    mfuStatus = false
                };

                return info;
            }

            info = new ExerciseAndValuesGraphicDto
            {
                exercises = exercises,
                activeMinutes = graphicValues,
                mfuStatus = true
            };

            return info;
        }

        public ExerciseListDto AddExercises(AddExerciseDto exercise)
        {
            var exerciseExisting = _bd.Exercises.Count(e => e.dateExercise == exercise.dateExercise && e.typeExercise == exercise.typeExercise &&
                                e.intensityExercise == exercise.intensityExercise && e.timeSpent == exercise.timeSpent);

            if (exerciseExisting > 0)
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

            var exerciseRecently = _bd.Exercises.FirstOrDefault(e => e.accountID == exercise.accountID && e.dateExercise == exercise.dateExercise
                                                                && e.typeExercise == exercise.typeExercise
                                                                && e.intensityExercise == exercise.intensityExercise
                                                                && e.timeSpent == exercise.timeSpent);

            ExerciseListDto info = GetExercise(exerciseRecently.exerciseID, exercise.dateExercise);

            return info;
        }

        public ExerciseListDto UpdateExercises(ExerciseListDto exercise)
        {
            var ex = _bd.Exercises.Find(exercise.exerciseID);

            if (ex == null)
            {
                throw new ExerciseNotFoundException();
            }

            if (ex.timeSpent < exercise.timeSpent || ex.timeSpent > exercise.timeSpent)
            {
                var previousTotal = _bd.ActiveMinutes.FirstOrDefault(e => e.dateExercise == ex.dateExercise);

                int currentTotal = previousTotal.totalTimeSpent - ex.timeSpent;
                int newTotal = currentTotal + exercise.timeSpent;

                previousTotal.totalTimeSpent = newTotal;

                _bd.ActiveMinutes.Update(previousTotal);

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

            ExerciseListDto info = GetExercise(exercise.exerciseID, exercise.dateExercise);

            return info;
        }

        public string DeleteExercise(Guid idexercise)
        {
            var ex = _bd.Exercises.Find(idexercise);

            if (ex == null)
            {
                throw new ExerciseNotFoundException();
            }

            Guid id = ex.accountID;
            DateOnly date = ex.dateExercise;

            var exerciseExisting = _bd.Exercises.Count(e => e.dateExercise == ex.dateExercise);
            var previousTotal = _bd.ActiveMinutes.FirstOrDefault(e => e.dateExercise == ex.dateExercise);

            if (exerciseExisting >= 2)
            {
                if (previousTotal != null)
                {
                    int currentTotal = previousTotal.totalTimeSpent - ex.timeSpent;
                    int newTotal = currentTotal;

                    previousTotal.totalTimeSpent = newTotal;

                    _bd.ActiveMinutes.Update(previousTotal);

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
                    _bd.ActiveMinutes.Remove(previousTotal);

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

        private void totalTimeSpentforDay(Guid id, DateOnly dateInitial, int timeSpent)
        {
            var infoGraphics = _bd.ActiveMinutes.FirstOrDefault(c => c.accountID == id && c.dateExercise == dateInitial);

            if (infoGraphics != null)
            {
                var value = infoGraphics.totalTimeSpent;

                infoGraphics.totalTimeSpent = value + timeSpent;

                _bd.ActiveMinutes.Update(infoGraphics);

                if (!Save())
                {
                    throw new UnstoredValuesException();
                }
            }
            else
            {
                ActiveMinutes dates = new ActiveMinutes
                {
                    accountID = id,
                    dateExercise = dateInitial,
                    totalTimeSpent = timeSpent
                };

                _bd.ActiveMinutes.Add(dates);

                if (!Save())
                {
                    throw new UnstoredValuesException();
                }
            }
        }

        private ExerciseListDto GetExercise(Guid id, DateOnly date)
        {
            var exercise = _bd.Exercises
            .FirstOrDefault(e => e.exerciseID == id && e.dateExercise == date);

            ExerciseListDto ex;

            if (exercise == null)
            {
                ex = _mapper.Map<ExerciseListDto>(exercise);
            }

            ex = _mapper.Map<ExerciseListDto>(exercise);

            return ex;
        }

        private static List<DateOnly> GetDatesInRange(DateOnly startDate, DateOnly endDate)
        {
            List<DateOnly> dates = new List<DateOnly>();

            if (endDate >= startDate)
            {
                for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    dates.Add(date);
                }
            }

            return dates;
        }
    }
}
