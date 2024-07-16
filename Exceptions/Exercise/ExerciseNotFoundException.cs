namespace AppVidaSana.Exceptions.Ejercicio
{
    public class ExerciseNotFoundException : Exception
    {
        public ExerciseNotFoundException() : base("Elemento(s) no encontrado.")
        {
        }
    }
}
