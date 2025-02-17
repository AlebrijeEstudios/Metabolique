﻿using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Sleep
{
    public class SleepHabitDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly dateRegister { get; set; }
         
        public int? sleepHours { get; set; }

        public string? perceptionOfRelaxation { get; set; }
    }
}
