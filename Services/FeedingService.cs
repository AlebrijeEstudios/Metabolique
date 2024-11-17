using AppVidaSana.Data;
using AppVidaSana.GraphicValues;
using AppVidaSana.Models.Dtos.Feeding_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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

        public async Task<UserFeedsDto> GetFeedingAsync(Guid userFeedID, CancellationToken cancellationToken)
        {
            var foodsConsumed = await _bd.FoodsConsumed.Where(e => e.userFeedID == userFeedID).ToListAsync(cancellationToken);
            var foodsConsumedMapped = _mapper.Map<List<FoodConsumedDto>>(foodsConsumed);

            var userFeeding = await _bd.UserFeeds.FirstOrDefaultAsync(e => e.userFeedID == userFeedID, cancellationToken);
            var userFeedingMapped = _mapper.Map<UserFeedsDto>(userFeeding);

            userFeedingMapped.foodsConsumed = foodsConsumedMapped;

            return userFeedingMapped;
        }

        public async Task<InfoGeneralFeedingDto> GetInfoGeneralFeedingAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            var userFeedingList = await _bd.UserFeeds.Where(e => e.accountID == accountID
                                                            && e.userFeedDate == date).ToListAsync(cancellationToken);


            throw new NotImplementedException();
        }

        public Task<UserFeedsDto> AddFeedingAsync(AddFeedingDto values, CancellationToken cancellationToken)
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
