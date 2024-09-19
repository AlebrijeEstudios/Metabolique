using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface ISideEffects
    {
        public SideEffectsListDto AddSideEffect(AddUpdateSideEffectDto values);
        public void UpdateSideEffect(AddUpdateSideEffectDto values);
        public void DeleteSideEffect(Guid id);
    }
}
