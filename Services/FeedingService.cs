using AppVidaSana.Data;
using AppVidaSana.GraphicValues;
using AppVidaSana.Models.Dtos.Feeding_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using AutoMapper;

namespace AppVidaSana.Services
{
    public class FeedingService : IFeeding
    {
        private readonly AppDbContext _bd;
        private ValidationValuesDB _validationValues;
        private DatesInRange _datesInRange;
        private readonly IMapper _mapper;

        public FeedingService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
            _validationValues = new ValidationValuesDB();
            _datesInRange = new DatesInRange();
        }

        public Task<UserFeedsDto> GetFeedingAsync(Guid userFeedID, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<InfoGeneralFeedingDto> GetInfoGeneralFeedingAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FeedingService> AddFeedingAsync(AddFeedingDto values, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserFeedsDto> UpdateFeedingAsync(UserFeedsDto values, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFeedingAsync(Guid userFeedID, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
