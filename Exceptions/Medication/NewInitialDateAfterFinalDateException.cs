﻿namespace AppVidaSana.Exceptions.Medication
{
    public class NewInitialDateAfterFinalDateException : Exception
    {
        public NewInitialDateAfterFinalDateException() : base("La nueva fecha inicial debe ser antes de la fecha final, inténtelo de nuevo.") { }
    }
}