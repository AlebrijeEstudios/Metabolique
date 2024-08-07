﻿using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Graphics_Dtos
{
    public class GraphicsValuesExerciseDto
    {
        [JsonRequired]  public Guid timeSpentID { get; set; }

        [JsonRequired]  public DateOnly dateExercise { get; set; }

        [JsonRequired]  public int totalTimeSpent { get; set; } 

    }
}
