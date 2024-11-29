using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Medications_Dtos;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.Monthly_Follows_Ups
{
    public class MFUsMedicationService : IMFUsMedications
    {
        private readonly AppDbContext _bd;
        private readonly Months _months;
        private readonly ValidationValuesDB _validationValues;

        public MFUsMedicationService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _months = new Months();
            _validationValues = new ValidationValuesDB();
        }

        public async Task<RetrieveResponsesMedicationsDto?> RetrieveAnswersAsync(Guid accountID, int month, int year, CancellationToken cancellationToken)
        {
            var monthStr = _months.VerifyExistMonth(month);

            RetrieveResponsesMedicationsDto? responses;

            var existMonth = await _bd.Months.FirstOrDefaultAsync(e => e.month == monthStr && e.year == year, cancellationToken);

            if (existMonth is null)
            {
                responses = null;
                return responses;
            }

            var mfuMedications = await _bd.MFUsMedication.FirstOrDefaultAsync(c => c.accountID == accountID 
                                                                              && c.monthID == existMonth.monthID, cancellationToken);

            if (mfuMedications is null)
            {
                responses = null;
                return responses;
            }

            var statusAdherence = await _bd.StatusAdherence.FindAsync(new object[] { mfuMedications.statusID }, cancellationToken);

            if (statusAdherence is null) { throw new UnstoredValuesException(); }

            responses = new RetrieveResponsesMedicationsDto
            {
                monthlyFollowUpID = mfuMedications.monthlyFollowUpID,
                month = existMonth.month,
                year = existMonth.year,
                answerQuestion1 = mfuMedications.answerQuestion1,
                answerQuestion2 = mfuMedications.answerQuestion2,
                answerQuestion3 = mfuMedications.answerQuestion3,
                answerQuestion4 = mfuMedications.answerQuestion4,
                statusAdherence = statusAdherence.statusAdherence
            };

            return responses;
        }

        public async Task<RetrieveResponsesMedicationsDto?> SaveAnswersAsync(SaveResponsesMedicationsDto values, CancellationToken cancellationToken)
        {
            var monthStr = _months.VerifyExistMonth(values.month);

            await ExistMonthAsync(monthStr, values.year, cancellationToken);

            var month = await _bd.Months.FirstOrDefaultAsync(e => e.month == monthStr
                                                             && e.year == values.year, cancellationToken);

            if (month is null) { throw new UnstoredValuesException(); }

            var answersExisting = await _bd.MFUsMedication.AnyAsync(e => e.accountID == values.accountID 
                                                                    && e.monthID == month.monthID, cancellationToken);

            if (answersExisting) { throw new RepeatRegistrationException(); }

            var statusAdherenceID = await GetStatusAdherenceID("Negativo", cancellationToken);

            if (!values.answerQuestion1 && values.answerQuestion2 && !values.answerQuestion3 && !values.answerQuestion4)
            {
                statusAdherenceID = await GetStatusAdherenceID("Positivo", cancellationToken);
            }

            MFUsMedication answers = new MFUsMedication
            {
                accountID = values.accountID,
                monthID = month.monthID,
                answerQuestion1 = values.answerQuestion1,
                answerQuestion2 = values.answerQuestion2,
                answerQuestion3 = values.answerQuestion3,
                answerQuestion4 = values.answerQuestion4,
                statusID = statusAdherenceID
            };

            _validationValues.ValidationValues(answers);

            _bd.MFUsMedication.Add(answers);

            if (!Save()) { throw new UnstoredValuesException(); }

            var responses = await RetrieveAnswersAsync(values.accountID, values.month, values.year, cancellationToken);

            return responses;

        }

        public async Task<RetrieveResponsesMedicationsDto?> UpdateAnswersAsync(UpdateResponsesMedicationsDto values, CancellationToken cancellationToken)
        {
            var mfuToUpdate = await _bd.MFUsMedication.FindAsync(new object[] { values.monthlyFollowUpID }, cancellationToken);

            if (mfuToUpdate == null) { throw new UnstoredValuesException(); }

            Guid statusAdherenceID = mfuToUpdate.statusID;

            if (!values.answerQuestion1 && values.answerQuestion2 && !values.answerQuestion3 && !values.answerQuestion4)
            {
                statusAdherenceID = await GetStatusAdherenceID("Positivo", cancellationToken);
            }
            else
            {
                statusAdherenceID = await GetStatusAdherenceID("Negativo", cancellationToken);
            }

            mfuToUpdate.answerQuestion1 = values.answerQuestion1;
            mfuToUpdate.answerQuestion2 = values.answerQuestion2;
            mfuToUpdate.answerQuestion3 = values.answerQuestion3;
            mfuToUpdate.answerQuestion4 = values.answerQuestion4;
            mfuToUpdate.statusID = statusAdherenceID;

            _validationValues.ValidationValues(mfuToUpdate);

            _bd.MFUsMedication.Update(mfuToUpdate);

            if (!Save()) { throw new UnstoredValuesException(); }

            var responses = await RetrieveAnswersAsync(mfuToUpdate.accountID, values.month, values.year, cancellationToken);

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

        private async Task ExistMonthAsync(string monthStr, int year, CancellationToken cancellationToken)
        {
            var existMonth = await _bd.Months.AnyAsync(e => e.month == monthStr && e.year == year, cancellationToken);

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

        private async Task<Guid> GetStatusAdherenceID(string status, CancellationToken cancellationToken)
        {
            var statusAdherence = await _bd.StatusAdherence.FirstOrDefaultAsync(e => e.statusAdherence == status, cancellationToken);

            if (statusAdherence is null) { throw new UnstoredValuesException(); }

            return statusAdherence.statusID;
        }
    }
}
