﻿using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Monthly_Follow_Ups
{
    public class MFUsMonths
    {
        [Key]
        public Guid monthID { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El campo mes es obligatorio")]
        public string month { get; set; } = null!;

        [Required(ErrorMessage = "El campo year es obligatorio")]
        public int year { get; set; }

        public ICollection<MFUsFood> foods { get; set; } = new List<MFUsFood>();

        public ICollection<MFUsMedication> medications { get; set; } = new List<MFUsMedication>();

        public ICollection<MFUsExercise> exercises { get; set; } = new List<MFUsExercise>();

        public ICollection<MFUsHabits> habits { get; set; } = new List<MFUsHabits>();

    }
}
