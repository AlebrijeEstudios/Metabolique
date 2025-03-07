﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Feeding
{
    public class UserFeedNutritionalValues
    {
        [Key]
        public Guid userFeedNutritionalValuesID { get; set; } = Guid.NewGuid();

        [ForeignKey("UserFeeds")]
        public Guid userFeedID { get; set; }

        [ForeignKey("NutritionalValues")]
        public Guid nutritionalValueID { get; set; }

        [Required(ErrorMessage = "El campo frecuencia de alimento es obligatorio")]
        public int MealFrequency { get; set; } 

        public UserFeeds? userFeeds { get; set; } 
        
        public NutritionalValues? nutritionalValues { get; set; } 
    }
}
