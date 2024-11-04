using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Medications_Dtos;

namespace AppVidaSana.Services.IServices.IMonthly_Follow_Ups
{
    public interface IMFUsMedications
    {
        RetrieveResponsesMedicationsDto SaveAnswers(SaveResponsesMedicationsDto values);

        RetrieveResponsesMedicationsDto RetrieveAnswers(Guid id, int month, int year);

        RetrieveResponsesMedicationsDto UpdateAnswers(UpdateResponsesMedicationsDto values);

        bool Save();

    }
}
