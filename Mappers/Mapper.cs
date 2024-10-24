using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;
using AppVidaSana.Models.Dtos.Habits_Dtos.ReturnInfoHabits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.Models.Exercises;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;

namespace AppVidaSana.Mappers
{
    public class Mapper : AutoMapper.Profile
    {
        public Mapper()
        {
            //ACCOUNT-PROFILE
            CreateMap<Account, InfoAccountDto>()
                .ForMember(dest => dest.birthDate, opt => opt.Ignore())
                .ForMember(dest => dest.sex, opt => opt.Ignore())
                .ForMember(dest => dest.stature, opt => opt.Ignore())
                .ForMember(dest => dest.weight, opt => opt.Ignore())
                .ForMember(dest => dest.protocolToFollow, opt => opt.Ignore());

            CreateMap<Profiles, InfoAccountDto>()
                .ForMember(dest => dest.accountID, opt => opt.Ignore())
                .ForMember(dest => dest.username, opt => opt.Ignore())
                .ForMember(dest => dest.email, opt => opt.Ignore());

            //FOOD
            CreateMap<MFUsFood, ResultsMFUsFoodDto>()
                .ForMember(dest => dest.month, opt => opt.Ignore())
                .ForMember(dest => dest.year, opt => opt.Ignore())
                .ForMember(dest => dest.totalPts, opt => opt.Ignore())
                .ForMember(dest => dest.classification, opt => opt.Ignore());

            CreateMap<FoodResults, ResultsMFUsFoodDto>()
                .ForMember(dest => dest.monthlyFollowUpID, opt => opt.Ignore())
                .ForMember(dest => dest.month, opt => opt.Ignore())
                .ForMember(dest => dest.year, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion1, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion2, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion3, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion4, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion6, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion7, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion8, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion9, opt => opt.Ignore());

            CreateMap<MFUsMonths, ResultsMFUsFoodDto>()
                .ForMember(dest => dest.monthlyFollowUpID, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion1, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion2, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion3, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion4, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion6, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion7, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion8, opt => opt.Ignore())
                .ForMember(dest => dest.answerQuestion9, opt => opt.Ignore())
                .ForMember(dest => dest.totalPts, opt => opt.Ignore())
                .ForMember(dest => dest.classification, opt => opt.Ignore());

            //EXERCISE
            CreateMap<Exercise, ExerciseListDto>().ReverseMap();
            CreateMap<ActiveMinutes, GraphicValuesExerciseDto>().ReverseMap();

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

            //HABITS
            CreateMap<DrinkHabit, GetDrinksConsumedDto>().ReverseMap();
            CreateMap<SleepHabit, GetHoursSleepConsumedDto>().ReverseMap();
            CreateMap<DrugsHabit, GetDrugsConsumedDto>().ReverseMap();
            CreateMap<SleepHabit, SleepHabitInfoDto>().ReverseMap();
            CreateMap<DrugsHabit, DrugsHabitInfoDto>().ReverseMap();

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
                .ForMember(dest => dest.monthlyFollowUpID, opt => opt.Ignore())
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

            //MEDICATION
            CreateMap<Times, TimeListDto>().ReverseMap();
            CreateMap<SideEffects, SideEffectsListDto>().ReverseMap();

        }
    }
}
