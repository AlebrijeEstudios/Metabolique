using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface ISideEffects
    {
        public Task<SideEffectsListDto> AddSideEffectAsync(AddSideEffectDto values, CancellationToken cancellationToken);

        public Task<SideEffectsListDto> UpdateSideEffectAsync(SideEffectsListDto values, CancellationToken cancellationToken);

        public Task<string> DeleteSideEffectAsync(Guid sideEffectID, CancellationToken cancellationToken);

        bool Save();
    }
}
