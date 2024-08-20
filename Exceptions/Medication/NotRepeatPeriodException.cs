namespace AppVidaSana.Exceptions.Medication
{
    public class NotRepeatPeriodException : Exception
    {
        public NotRepeatPeriodException() : base("El periodo que coloco ya fue registrado con anterioridad para este medicamento.") { }

    }
}
