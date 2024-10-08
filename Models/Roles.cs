﻿using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models
{
    public class Roles
    {
        [Key]
        public Guid roleID { get; set; } = Guid.NewGuid();

        public string role { get; set; } = null!;

        public ICollection<Account> account { get; set; } = new List<Account>();
    }
}
