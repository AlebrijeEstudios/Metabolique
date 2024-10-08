﻿using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;
using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Exercise_Dtos
{
    public class ExerciseAndValuesGraphicDto
    {
        [JsonRequired] public List<ExerciseListDto> exercises { get; set; } = null!;

        [JsonRequired] public List<GraphicValuesExerciseDto> activeMinutes { get; set; } = null!;

        [JsonRequired] public bool mfuStatus { get; set; }

    }
}
