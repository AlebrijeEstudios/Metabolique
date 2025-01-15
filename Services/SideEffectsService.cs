using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services
{
    public class SideEffectsService : ISideEffects
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public SideEffectsService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public async Task<SideEffectsListDto> AddSideEffectAsync(AddSideEffectDto values, CancellationToken cancellationToken)
        {
            var sideEffectExist = await _bd.SideEffects.FirstOrDefaultAsync(e => e.accountID == values.accountID
                                                                            && e.dateSideEffects == values.date
                                                                            && e.description == values.description, cancellationToken);

            if (sideEffectExist is not null) { throw new RepeatRegistrationException(); }

            SideEffects sideEffects = new SideEffects
            {
                accountID = values.accountID,
                dateSideEffects = values.date,
                initialTime = values.initialTime,
                finalTime = values.finalTime,
                description = values.description
            };

            ValidationValuesDB.ValidationValues(sideEffects);

            _bd.SideEffects.Add(sideEffects);

            if (!Save()) { throw new UnstoredValuesException(); }

            var sideEffectMapped = _mapper.Map<SideEffectsListDto>(sideEffects);

            return sideEffectMapped;
        }

        public async Task<SideEffectsListDto> UpdateSideEffectAsync(SideEffectsListDto values, CancellationToken cancellationToken)
        {
            var sideEffectToUpdate = await _bd.SideEffects.FindAsync(new object[] { values.sideEffectID }, cancellationToken);

            if (sideEffectToUpdate is null) { throw new UnstoredValuesException(); }

            sideEffectToUpdate.initialTime = values.initialTime;
            sideEffectToUpdate.finalTime = values.finalTime;
            sideEffectToUpdate.description = values.description;

            ValidationValuesDB.ValidationValues(sideEffectToUpdate);

            if (!Save()) { throw new UnstoredValuesException(); }

            var sideEffectMapped = _mapper.Map<SideEffectsListDto>(sideEffectToUpdate);

            return sideEffectMapped;
        }

        public async Task<string> DeleteSideEffectAsync(Guid sideEffectID, CancellationToken cancellationToken)
        {
            var sideEffectToDelete = await _bd.SideEffects.FindAsync(new object[] { sideEffectID }, cancellationToken);

            if (sideEffectToDelete is null)
            {
                return "Este registro no existe, inténtelo de nuevo.";
            }

            _bd.SideEffects.Remove(sideEffectToDelete);

            if (!Save()) { throw new UnstoredValuesException(); }

            return "Se ha eliminado correctamente.";
        }

        public bool Save()
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
