﻿using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.ReturnInfoHabits
{
    public class GraphicValuesHabitSleepDto
    {
        [JsonRequired] public DateOnly date { get; set; }

        [JsonRequired] public int? value { get; set; }

    }
}