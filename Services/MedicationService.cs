using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AppVidaSana.Services
{
    public class MedicationService : IMedication
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public MedicationService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }
        public List<ReturnInfoMedicationDto> GetAccount(Guid id, DateOnly date)
        {
            throw new NotImplementedException();
        }

        public List<ReturnInfoMedicationDto> AddMedication(AddUpdateMedicationUseDto medication)
        {
            var medicationExisting = _bd.Medications.Count(e => e.nameMedication == medication.nameMedication
                                        && e.dose == medication.dose && e.initialFrec == medication.initialFrec 
                                        && e.finalFrec == medication.finalFrec && e.dailyFrec == medication.dailyFrec);

            if (medicationExisting > 0)
            {
                throw new RepeatRegistrationException();
            }

           
            throw new NotImplementedException();
        }
        public List<ReturnInfoMedicationDto> UpdateMedication(AddUpdateMedicationUseDto values)
        {
            throw new NotImplementedException();
        }

        public List<ReturnInfoMedicationDto> DeleteMedication(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public static List<DateOnly> GetDatesInRange(DateOnly startDate, DateOnly endDate)
        {
            List<DateOnly> dates = new List<DateOnly>();

            if (endDate >= startDate)
            {
                for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    dates.Add(date);
                }
            }

            return dates;
        }


    }
}
