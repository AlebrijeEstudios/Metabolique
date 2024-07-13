using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IEjercicio
    {
        List<ListaEjerciciosDto> ObtenerEjercicios(Guid id, DateOnly fecha);

        string AñadirEjercicio(AñadirEjercicioDto ejercicio);

        string ActualizarEjercicio(Guid idejercicio, ListaEjerciciosDto ejerciciodto);

        string EliminarEjercicio(Guid idejercicio);

        bool Guardar();
    }
}
