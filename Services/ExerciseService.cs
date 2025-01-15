using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Exercise_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;
using AppVidaSana.Models.Exercises;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using AppVidaSana.ValidationValues;
using AppVidaSana.GraphicValues;

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

        public async Task<List<ExerciseDto>> GetExercisesAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            var exercises = await _bd.Exercises.Where(e => e.accountID == accountID 
                                                      && e.dateExercise == date).ToListAsync(cancellationToken);

            var exercisesMapped = _mapper.Map<List<ExerciseDto>>(exercises);

            return exercisesMapped;
        }

        public async Task<InfoGeneralExerciseDto> GetInfoGeneralExercisesAsync(Guid accountID, DateOnly date,
                                                                               CancellationToken cancellationToken)
        {
            List<ExerciseDto> exercises = await GetExercisesAsync(accountID, date, cancellationToken);

            List<ActiveMinutesExerciseDto> activeMinutes = await GetActiveMinutesAsync(accountID, date, cancellationToken);

            CultureInfo cultureInfo = new CultureInfo("es-ES");

            var monthExist = await _bd.Months.FirstOrDefaultAsync(e => e.month == date.ToString("MMMM", cultureInfo)
                                                                  && e.year == Convert.ToInt32(date.ToString("yyyy")), 
                                                                  cancellationToken);
           
            if (monthExist is null)
            {
                return GeneratedInfoGeneralExercise(exercises, activeMinutes, false);
            }
            
            var mfuExist = await _bd.MFUsExercise.AnyAsync(e => e.accountID == accountID
                                                           && e.monthID == monthExist.monthID, cancellationToken);

            if (!mfuExist)
            {
                return GeneratedInfoGeneralExercise(exercises, activeMinutes, false);
            }

            return GeneratedInfoGeneralExercise(exercises, activeMinutes, true);
        }

        public async Task<ExerciseDto> AddExerciseAsync(AddExerciseDto values, CancellationToken cancellationToken)
        {
            var exerciseExisting = await _bd.Exercises.FirstOrDefaultAsync(e => e.accountID == values.accountID
                                                                           && e.dateExercise == values.dateExercise
                                                                           && e.typeExercise == values.typeExercise
                                                                           && e.intensityExercise == values.intensityExercise
                                                                           && e.timeSpent == values.timeSpent, cancellationToken);

            if (exerciseExisting is not null) { throw new RepeatRegistrationException(); }

            Exercise exercise = new Exercise
            {
                accountID = values.accountID,
                dateExercise = values.dateExercise,
                typeExercise = values.typeExercise,
                intensityExercise = values.intensityExercise,
                timeSpent = values.timeSpent
            };

            ValidationValuesDB.ValidationValues(exercise);

            _bd.Exercises.Add(exercise);

            if (!Save()) { throw new UnstoredValuesException(); }
            
            await totalTimeSpentforDayAsync(exercise.accountID, exercise.dateExercise, exercise.timeSpent, cancellationToken);

            var exerciseMapped = _mapper.Map<ExerciseDto>(exercise);

            return exerciseMapped;
        }

        public async Task<ExerciseDto> UpdateExerciseAsync(ExerciseDto values, CancellationToken cancellationToken)
        {
            var exercise = await _bd.Exercises.FindAsync(new object[] { values.exerciseID }, cancellationToken);

            if (exercise is null) { throw new ExerciseNotFoundException(); }

            if (exercise.timeSpent < values.timeSpent || exercise.timeSpent > values.timeSpent)
            {
                var previousTotal = await _bd.ActiveMinutes.FirstOrDefaultAsync(e => 
                                                                                e.dateExercise == exercise.dateExercise,
                                                                                cancellationToken);

                if (previousTotal is null) { throw new UnstoredValuesException(); }

                int currentTotal = previousTotal.totalTimeSpent - exercise.timeSpent;
                int newTotal = currentTotal + values.timeSpent;

                previousTotal.totalTimeSpent = newTotal;

                ValidationValuesDB.ValidationValues(previousTotal);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            exercise.typeExercise = values.typeExercise;
            exercise.intensityExercise = values.intensityExercise;
            exercise.timeSpent = values.timeSpent;

            ValidationValuesDB.ValidationValues(exercise);

            if (!Save()) { throw new UnstoredValuesException(); }

            var exerciseMapped = _mapper.Map<ExerciseDto>(exercise);

            return exerciseMapped;
        }

        public async Task<string> DeleteExerciseAsync(Guid exerciseID, CancellationToken cancellationToken)
        {
            var exerciseExisting = await _bd.Exercises.FindAsync(new object[] { exerciseID }, cancellationToken);

            if (exerciseExisting is null) { throw new ExerciseNotFoundException(); }

            var exerciseForDate = await _bd.Exercises.CountAsync(e => 
                                                                 e.dateExercise == exerciseExisting.dateExercise, 
                                                                 cancellationToken);

            var previousTotal = await _bd.ActiveMinutes.FirstOrDefaultAsync(e => 
                                                                            e.dateExercise == exerciseExisting.dateExercise,
                                                                            cancellationToken);

            if (previousTotal is null) { throw new UnstoredValuesException(); }

            if (exerciseForDate >= 2)
            {
                int currentTotal = previousTotal.totalTimeSpent - exerciseExisting.timeSpent;

                int newTotal = currentTotal;

                previousTotal.totalTimeSpent = newTotal;

                ValidationValuesDB.ValidationValues(previousTotal);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            if (exerciseForDate == 1)
            {
                _bd.ActiveMinutes.Remove(previousTotal);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            _bd.Exercises.Remove(exerciseExisting);

            if (!Save()) { throw new UnstoredValuesException(); }

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

        private static InfoGeneralExerciseDto GeneratedInfoGeneralExercise(List<ExerciseDto> exercises, 
                                                                    List<ActiveMinutesExerciseDto> activeMinutes, bool status)
        {
            InfoGeneralExerciseDto infoGeneral = new InfoGeneralExerciseDto
            {
                exercises = exercises,
                activeMinutes = activeMinutes,
                mfuStatus = status
            };

            return infoGeneral;
        }

        private async Task<List<ActiveMinutesExerciseDto>> GetActiveMinutesAsync(Guid accountID, 
                                                                                 DateOnly date, CancellationToken cancellationToken)
        {
            DateOnly dateFinal = date.AddDays(-6);

            var dates = DatesInRange.GetDatesInRange(dateFinal, date);

            var values = await _bd.ActiveMinutes.Where(c => c.accountID == accountID
                                                       && dates.Contains(c.dateExercise)).ToListAsync(cancellationToken);

            var activeMinutes = dates.Select(date => new ActiveMinutesExerciseDto
                                {
                                    date = date,
                                    value = values.FirstOrDefault(e => e.dateExercise == date)?.totalTimeSpent ?? 0
                                }).ToList();

            activeMinutes = activeMinutes.OrderBy(x => x.date).ToList();

            return activeMinutes;
        }

        private async Task totalTimeSpentforDayAsync(Guid accountID, DateOnly date, 
                                                     int timeSpent, CancellationToken cancellationToken)
        {
            var activeMinutesExisting = await _bd.ActiveMinutes.FirstOrDefaultAsync(c => c.accountID == accountID 
                                                                                    && c.dateExercise == date, cancellationToken);

            if (activeMinutesExisting is not null)
            {
                var value = activeMinutesExisting.totalTimeSpent;

                activeMinutesExisting.totalTimeSpent = value + timeSpent;

                ValidationValuesDB.ValidationValues(activeMinutesExisting);

                if (!Save()) { throw new UnstoredValuesException(); }
            }
            else
            {
                ActiveMinutes activeMinutes = new ActiveMinutes
                {
                    accountID = accountID,
                    dateExercise = date,
                    totalTimeSpent = timeSpent
                };

                ValidationValuesDB.ValidationValues(activeMinutes);

                _bd.ActiveMinutes.Add(activeMinutes);

                if (!Save()) { throw new UnstoredValuesException(); }
            }
        }
    }
}
