using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Medications_Dtos;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Services.Monthly_Follows_Ups
{
    public class MFUsMedicationService : IMFUsMedications
    {
        private readonly AppDbContext _bd;
        private Months _months;

        public MFUsMedicationService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _months = new Months();
        }

        public RetrieveResponsesMedicationsDto RetrieveAnswers(Guid id, int month, int year)
        {
            var monthStr = _months.VerifyExistMonth(month);

            RetrieveResponsesMedicationsDto responses;

            var existMonth = _bd.Months.FirstOrDefault(e => e.month == monthStr && e.year == year);

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
                month = existMonth.month,
                year = existMonth.year,
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
            var monthStr = _months.VerifyExistMonth(values.month);

            ExistMonth(monthStr, values.year);

            Guid monthID = _bd.Months.FirstOrDefault(e => e.month == monthStr && e.year == values.year).monthID;

            var answersExisting = _bd.MFUsMedication.Any(e => e.accountID == values.accountID && e.monthID == monthID);

            if (answersExisting) { throw new RepeatRegistrationException(); }

            var accountExisting = _bd.Accounts.Find(values.accountID);

            if (accountExisting == null) { throw new UserNotFoundException(); }

            var statusID = _bd.StatusAdherence.FirstOrDefault(e => e.statusAdherence == "Negativo").statusID;

            if (!values.answerQuestion1 && values.answerQuestion2 && !values.answerQuestion3 && !values.answerQuestion4)
            {
                statusID = _bd.StatusAdherence.FirstOrDefault(e => e.statusAdherence == "Positivo").statusID;
            }

            MFUsMedication answers = new MFUsMedication
            {
                accountID = values.accountID,
                monthID = monthID,
                answerQuestion1 = values.answerQuestion1,
                answerQuestion2 = values.answerQuestion2,
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

            Guid statusID = mfuToUpdate.statusID;

            if (!values.answerQuestion1 && values.answerQuestion2 && !values.answerQuestion3 && !values.answerQuestion4)
            {
                statusID = _bd.StatusAdherence.FirstOrDefault(e => e.statusAdherence == "Positivo").statusID;
            }
            else
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

        private void ExistMonth(string monthStr, int year)
        {
            var existMonth = _bd.Months.Any(e => e.month == monthStr && e.year == year);

            if (!existMonth)
            {
                MFUsMonths month = new MFUsMonths
                {
                    month = monthStr,
                    year = year
                };

                _bd.Months.Add(month);

                if (!Save()) { throw new UnstoredValuesException(); }
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
