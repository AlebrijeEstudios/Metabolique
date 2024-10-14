using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using AutoMapper;

namespace AppVidaSana.Services.Monthly_Follows_Ups
{
    public class MFUsFoodService : IMFUsFood
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public MFUsFoodService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public ResultsMFUsFoodDto RetrieveAnswers(Guid accountID, int month, int year)
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

            ResultsMFUsFoodDto results;

            var existMonth = _bd.Months.FirstOrDefault(e => e.month == months[month] && e.year == year);

            if (existMonth == null)
            {
                results = null;
                return results;
            }

            var mfuFood = _bd.MFUsFood.FirstOrDefault(c => c.accountID == accountID && c.monthID == existMonth.monthID);

            if (mfuFood == null)
            {
                results = null;
                return results;
            }

            var mfuFoodResults = _bd.ResultsFood.FirstOrDefault(c => c.monthlyFollowUpID == mfuFood.monthlyFollowUpID);

            if (mfuFoodResults == null)
            {
                results = null;
                return results;
            }

            results = _mapper.Map<ResultsMFUsFoodDto>(mfuFood);
            results = _mapper.Map(existMonth, results);
            _mapper.Map(mfuFoodResults, results);

            return results;
        }

        public ResultsMFUsFoodDto SaveAnswers(AnswersMFUsFoodDto values)
        {
            throw new NotImplementedException();
        }

        public ResultsMFUsFoodDto UpdateAnswers(UpdateAnswersMFUsFoodDto values)
        {
            throw new NotImplementedException();
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
