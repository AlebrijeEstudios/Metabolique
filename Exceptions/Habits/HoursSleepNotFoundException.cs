namespace AppVidaSana.Exceptions.Habits
{
    public class HoursSleepNotFoundException : Exception
    {
        public HoursSleepNotFoundException() : base("No existe información sobre las horas de sueño en los últimos 7 días.")
        {
        }
    }
}
