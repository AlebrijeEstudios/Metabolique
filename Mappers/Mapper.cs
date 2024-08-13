using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep_And_Drugs;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.Models.Exercises;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using AutoMapper;

namespace AppVidaSana.Mappers
{
    public class Mapper : AutoMapper.Profile
    {
        public Mapper()
        {
            CreateMap<Exercise, ExerciseListDto>().ReverseMap();
            CreateMap<ActiveMinutes, GraphicsValuesExerciseDto>().ReverseMap();
            CreateMap<DrinkHabit, GetDrinksConsumedDto>().ReverseMap();
            CreateMap<SleepHabit, GetSleepingHoursDto>().ReverseMap();
            CreateMap<DrugsHabit, GetDrugsConsumedDto>().ReverseMap();

            CreateMap<SleepHabit, ReturnSleepHoursAndDrugsConsumedDto>()
            .ForMember(dest => dest.drugsHabitID, opt => opt.Ignore())
            .ForMember(dest => dest.cigarettesSmoked, opt => opt.Ignore())
            .ForMember(dest => dest.predominantEmotionalState, opt => opt.Ignore());

            CreateMap<DrugsHabit, ReturnSleepHoursAndDrugsConsumedDto>()
            .ForMember(dest => dest.dateRegister, opt => opt.Ignore())
            .ForMember(dest => dest.sleepHabitID, opt => opt.Ignore())
            .ForMember(dest => dest.sleepHours, opt => opt.Ignore())
            .ForMember(dest => dest.perceptionOfRelaxation, opt => opt.Ignore());

            CreateMap<MFUsExercise, RetrieveResponsesExerciseDto>()
            .ForMember(dest => dest.month, opt => opt.Ignore())
            .ForMember(dest => dest.year, opt => opt.Ignore())
            .ForMember(dest => dest.actWalking, opt => opt.Ignore())
            .ForMember(dest => dest.actModerate, opt => opt.Ignore())
            .ForMember(dest => dest.actVigorous, opt => opt.Ignore())
            .ForMember(dest => dest.totalMET, opt => opt.Ignore())
            .ForMember(dest => dest.sedentaryBehavior, opt => opt.Ignore())
            .ForMember(dest => dest.levelAF, opt => opt.Ignore());

            CreateMap<ExerciseResults, RetrieveResponsesExerciseDto>()
             .ForMember(dest => dest.monthlyFollowUpID, opt => opt.Ignore())
            .ForMember(dest => dest.month, opt => opt.Ignore())
            .ForMember(dest => dest.year, opt => opt.Ignore())
            .ForMember(dest => dest.question1, opt => opt.Ignore())
            .ForMember(dest => dest.question2, opt => opt.Ignore())
            .ForMember(dest => dest.question3, opt => opt.Ignore())
            .ForMember(dest => dest.question3, opt => opt.Ignore())
            .ForMember(dest => dest.question4, opt => opt.Ignore())
            .ForMember(dest => dest.question5, opt => opt.Ignore())
            .ForMember(dest => dest.question6, opt => opt.Ignore())
            .ForMember(dest => dest.question7, opt => opt.Ignore());

            CreateMap<MFUsMonths, RetrieveResponsesExerciseDto>()
            .ForMember(dest => dest.monthlyFollowUpID, opt => opt.Ignore())
            .ForMember(dest => dest.question1, opt => opt.Ignore())
            .ForMember(dest => dest.question2, opt => opt.Ignore())
            .ForMember(dest => dest.question3, opt => opt.Ignore())
            .ForMember(dest => dest.question4, opt => opt.Ignore())
            .ForMember(dest => dest.question5, opt => opt.Ignore())
            .ForMember(dest => dest.question6, opt => opt.Ignore())
            .ForMember(dest => dest.question7, opt => opt.Ignore())
            .ForMember(dest => dest.actWalking, opt => opt.Ignore())
            .ForMember(dest => dest.actModerate, opt => opt.Ignore())
            .ForMember(dest => dest.actVigorous, opt => opt.Ignore())
            .ForMember(dest => dest.totalMET, opt => opt.Ignore())
            .ForMember(dest => dest.sedentaryBehavior, opt => opt.Ignore())
            .ForMember(dest => dest.levelAF, opt => opt.Ignore());

            CreateMap<MFUsHabits, RetrieveResponsesHabitsDto>()
            .ForMember(dest => dest.month, opt => opt.Ignore())
            .ForMember(dest => dest.year, opt => opt.Ignore())
            .ForMember(dest => dest.resultComponent1, opt => opt.Ignore())
            .ForMember(dest => dest.resultComponent2, opt => opt.Ignore())
            .ForMember(dest => dest.resultComponent3, opt => opt.Ignore())
            .ForMember(dest => dest.resultComponent4, opt => opt.Ignore())
            .ForMember(dest => dest.resultComponent5, opt => opt.Ignore())
            .ForMember(dest => dest.resultComponent6, opt => opt.Ignore())
            .ForMember(dest => dest.resultComponent7, opt => opt.Ignore())
            .ForMember(dest => dest.globalClassification, opt => opt.Ignore())
            .ForMember(dest => dest.classification, opt => opt.Ignore());

            CreateMap<HabitsResults, RetrieveResponsesHabitsDto>()
                .ForMember(dest => dest.monthlyFollowUpID, opt => opt.Ignore())
                .ForMember(dest => dest.month, opt => opt.Ignore())
                .ForMember(dest => dest.year, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion1, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion2, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion3, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion4, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5a, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5b, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5c, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5d, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5e, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5f, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5g, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5h, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5i, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5j, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion6, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion7, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion8, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion9, opt => opt.Ignore());

            CreateMap<MFUsMonths, RetrieveResponsesHabitsDto>()
                .ForMember(dest => dest.answerQuestion1, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion2, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion3, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion4, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5a, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5b, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5c, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5d, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5e, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5f, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5g, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5h, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5i, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion5j, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion6, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion7, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion8, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion9, opt => opt.Ignore())
                .ForMember(dest => dest.monthlyFollowUpID, opt => opt.Ignore())
                .ForMember(dest => dest.resultComponent1, opt => opt.Ignore())
                .ForMember(dest => dest.resultComponent2, opt => opt.Ignore())
                .ForMember(dest => dest.resultComponent3, opt => opt.Ignore())
                .ForMember(dest => dest.resultComponent4, opt => opt.Ignore())
                .ForMember(dest => dest.resultComponent5, opt => opt.Ignore())
                .ForMember(dest => dest.resultComponent6, opt => opt.Ignore())
                .ForMember(dest => dest.resultComponent7, opt => opt.Ignore())
                .ForMember(dest => dest.globalClassification, opt => opt.Ignore())
                .ForMember(dest => dest.classification, opt => opt.Ignore());

            CreateMap<MFUsHabits, SaveResponsesHabitsDto>().ReverseMap();
            CreateMap<HabitsResults, SaveResultsHabitsDto>().ReverseMap();
            CreateMap<Times, TimeListDto>().ReverseMap();

        }
    }
}
