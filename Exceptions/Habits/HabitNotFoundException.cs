namespace AppVidaSana.Exceptions.Habits
{
    public class HabitNotFoundException : Exception
    { 
        public HabitNotFoundException(string message) : base(message) { }
    }
}
