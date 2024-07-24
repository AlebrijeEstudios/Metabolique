namespace AppVidaSana.Exceptions.Habits
{
    public class HabitNotFoundException : Exception
    {
        public HabitNotFoundException() : base("Elemento no encontrado, inténtelo de nuevo."){ }

        public HabitNotFoundException(string message) : base(message) { }
    }
}
