using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Medications_Dtos;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Services.Monthly_Follows_Ups
{
    public class MFUsMedicationService : IMFUsMedications
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public MFUsMedicationService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public RetrieveResponsesMedicationsDto RetrieveAnswers(Guid id, int month, int year)
        {
            var months = new Dictionary<int, string>
            {
                { 1, "Enero" },
                { 2, "Febrero" },
                { 3, "Marzo" },
                { 4, "Abril" },
                { 5, "Mayo" },
                { 6, "Junio" },
                { 7, "Julio" },
                { 8, "Agosto" },
                { 9, "Septiembre" },
                { 10, "Octubre" },
                { 11, "Noviembre" },
                { 12, "Diciembre" }
            };

            var getMonth = months.ContainsKey(month) ? months[month] : "Mes no existente";

            if (getMonth == "Mes no existente") { throw new UnstoredValuesException(); }

            RetrieveResponsesMedicationsDto responses;

            var existMonth = _bd.Months.FirstOrDefault(e => e.month == months[month] && e.year == year);

            if (existMonth == null)
            {
                responses = null;
                return responses;
            }

            var mfuMedications = _bd.MFUsMedication.FirstOrDefault(c => c.accountID == id && c.monthID == existMonth.monthID);

            if (mfuMedications == null)
            {
                responses = null;
                return responses;
            }

            responses = new RetrieveResponsesMedicationsDto
            {
                monthlyFollowUpID = mfuMedications.monthlyFollowUpID,
                answerQuestion1 = mfuMedications.answerQuestion1,
                answerQuestion2 = mfuMedications.answerQuestion2,
                answerQuestion3 = mfuMedications.answerQuestion3,
                answerQuestion4 = mfuMedications.answerQuestion4,
                statusAdherence = _bd.StatusAdherence.Find(mfuMedications.statusID).statusAdherence
            };

            return responses;
        }

        public RetrieveResponsesMedicationsDto SaveAnswers(SaveResponsesMedicationsDto values)
        {
            var months = new Dictionary<int, string>
            {
                { 1, "Enero" },
                { 2, "Febrero" },
                { 3, "Marzo" },
                { 4, "Abril" },
                { 5, "Mayo" },
                { 6, "Junio" },
                { 7, "Julio" },
                { 8, "Agosto" },
                { 9, "Septiembre" },
                { 10, "Octubre" },
                { 11, "Noviembre" },
                { 12, "Diciembre" }
            };

            var getMonth = months.ContainsKey(values.month) ? months[values.month] : "Mes no existente";

            if (getMonth == "Mes no existente") { throw new UnstoredValuesException(); }

            var existMonth = _bd.Months.Any(e => e.month == months[values.month] && e.year == values.year);

            if (!existMonth)
            {
                MFUsMonths month = new MFUsMonths
                {
                    month = months[values.month],
                    year = values.year
                };

                _bd.Months.Add(month);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            Guid monthID = _bd.Months.FirstOrDefault(e => e.month == months[values.month] && e.year == values.year).monthID;

            var answersExisting = _bd.MFUsMedication.Any(e => e.accountID == values.accountID && e.monthID == monthID);

            if (answersExisting) { throw new RepeatRegistrationException(); }

            var accountExisting = _bd.Accounts.Find(values.accountID);

            if (accountExisting == null) { throw new UserNotFoundException(); }

            var statusID = _bd.StatusAdherence.FirstOrDefault(e => e.statusAdherence == "Negativo").statusID;

            if(!values.answerQuestion1 && values.answerQuestion2 && !values.answerQuestion3 && !values.answerQuestion4)
            {
                statusID = _bd.StatusAdherence.FirstOrDefault(e => e.statusAdherence == "Positivo").statusID;
            }

            MFUsMedication answers = new MFUsMedication
            {
                accountID = values.accountID,
                answerQuestion1 = values.answerQuestion1,
                answerQuestion2 =  values.answerQuestion2,
                answerQuestion3 = values.answerQuestion3,
                answerQuestion4 = values.answerQuestion4,
                statusID = statusID
            };

            ValidationSaveAnswers(answers);

            _bd.MFUsMedication.Add(answers);

            if (!Save()) { throw new UnstoredValuesException(); }

            var answersRecentlyAdd = _bd.MFUsMedication.FirstOrDefault(e => e.accountID == values.accountID && e.monthID == monthID);

            var responses = RetrieveAnswers(values.accountID, values.month, values.year);

            return responses;

        }

        public RetrieveResponsesMedicationsDto UpdateAnswers(UpdateResponsesMedicationsDto values)
        {
            var mfuToUpdate = _bd.MFUsMedication.Find(values.monthlyFollowUpID);

            if (mfuToUpdate == null) { throw new UnstoredValuesException(); }

            var statusID = _bd.StatusAdherence.FirstOrDefault(e => e.statusAdherence == "Negativo").statusID;

            if (!(!values.answerQuestion1 && values.answerQuestion2 && !values.answerQuestion3 && !values.answerQuestion4))
            {
                statusID = _bd.StatusAdherence.FirstOrDefault(e => e.statusAdherence == "Negativo").statusID;
            }

            mfuToUpdate.answerQuestion1 = values.answerQuestion1;
            mfuToUpdate.answerQuestion2 = values.answerQuestion2;
            mfuToUpdate.answerQuestion3 = values.answerQuestion3;
            mfuToUpdate.answerQuestion4 = values.answerQuestion4;
            mfuToUpdate.statusID = statusID;

            ValidationSaveAnswers(mfuToUpdate);

            _bd.MFUsMedication.Update(mfuToUpdate);

            if (!Save()) { throw new UnstoredValuesException(); }

            var responses = RetrieveAnswers(mfuToUpdate.accountID, values.month, values.year);

            return responses;
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

        private void ValidationSaveAnswers(MFUsMedication mfus)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(mfus, null, null);

            if (!Validator.TryValidateObject(mfus, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }
        }

    }
}
