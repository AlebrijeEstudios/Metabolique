﻿namespace AppVidaSana.Exceptions.Medication
{
    public class NewFinalDateBeforeInitialDateException : Exception
    {
        public NewFinalDateBeforeInitialDateException() : base("La nueva fecha final debe ser despues de la fecha inicial, int&eacute;ntelo de nuevo.") { }
    }
}
