using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface ISideEffects
    {
        public SideEffectsListDto AddSideEffect(AddSideEffectDto values);

        public SideEffectsListDto UpdateSideEffect(SideEffectsListDto values);

        public string DeleteSideEffect(Guid id);
    }
}
