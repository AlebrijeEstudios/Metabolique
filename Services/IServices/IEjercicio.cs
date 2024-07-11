using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IEjercicio
    {
        List<ListaEjerciciosDto> ObtenerEjercicios(ObtenerListaEjerciciosDto datos);

        string AñadirEjercicio(AñadirEjercicioDto ejercicio);

        string ActualizarEjercicio(Guid idejercicio, ListaEjerciciosDto ejerciciodto);

        string EliminarEjercicio(Guid idejercicio);

        bool Guardar();
    }
}
