using AppVidaSana.Data;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Services.IServices;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Services
{
    public class EjercicioService : IEjercicio
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public EjercicioService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;

        }

        public List<ListaEjerciciosDto> ObtenerEjercicios(Guid id, DateOnly fecha)
        {
            var ejercicios = _bd.Ejercicios
            .Where(e => e.id == id && e.fecha == fecha)
            .ToList();

            if(ejercicios.Count == 0)
            {
                throw new EjercicioNotFoundException();
            }

            var ejerciciosDto = _mapper.Map<List<ListaEjerciciosDto>>(ejercicios);

            return ejerciciosDto;
        }

        public string ActualizarEjercicio(Guid idejercicio, ListaEjerciciosDto ejerciciodto)
        {
            var eje = _bd.Ejercicios.Find(idejercicio);

            if (eje == null)
            {
                throw new EjercicioNotFoundException();
            }

            eje.tipo = ejerciciodto.tipo;
            eje.intensidad = ejerciciodto.intensidad;
            eje.tiempo = ejerciciodto.tiempo;


            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(eje, null, null);

            if (!Validator.TryValidateObject(eje, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                throw new ErrorDatabaseException(errors);
            }

            _bd.Ejercicios.Update(eje);
            Guardar();
            return "Actualización completada";

        }

        public string AñadirEjercicio(AñadirEjercicioDto ejercicio)
        {
            var eje = _bd.Cuentas.Find(ejercicio.id);

            if (eje == null)
            {
                return "No se guardaron los datos, intentelo de nuevo";
            }

            Ejercicio ej = new Ejercicio
            {
                id = ejercicio.id,
                fecha = ejercicio.fecha,
                tipo = ejercicio.tipo,
                intensidad = ejercicio.intensidad,
                tiempo = ejercicio.tiempo,
                cuenta = null
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(ej, null, null);

            if (!Validator.TryValidateObject(ej, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                throw new ErrorDatabaseException(errors);
            }

            _bd.Ejercicios.Add(ej);
            Guardar();
            return "Los datos han sido guardados correctamente";
        }

        public string EliminarEjercicio(Guid idejercicio)
        {
            var ejercicio = _bd.Ejercicios.Find(idejercicio);
            if (ejercicio == null)
            {
                throw new EjercicioNotFoundException();
            }

            _bd.Ejercicios.Remove(ejercicio);
            Guardar();
            return "Se ha eliminado correctamente.";
        }

        public bool Guardar()
        {
            try
            {
                return _bd.SaveChanges() >= 0;
            }
            catch (Exception)
            {
                return false;

            }
        } 
    }
}
