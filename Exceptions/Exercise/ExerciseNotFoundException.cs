namespace AppVidaSana.Exceptions.Exercise
{
    public class ExerciseNotFoundException : Exception
    {
        public ExerciseNotFoundException() : base("Elemento(s) no encontrado(s).")
        {
        }
    }
}
