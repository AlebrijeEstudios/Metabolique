﻿using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;

namespace AppVidaSana.Services.IServices.IMonthly_Follow_Ups
{
    public interface IMFUsHabits
    {
        SaveResultsDto SaveAnswers(SaveResponsesHabitsDto res);

        public string SaveResults(SaveResultsDto res);

        RetrieveResponsesHabitsDto RetrieveAnswers(Guid id, string month, int year);

        bool Save();

    }
}