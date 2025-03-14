﻿using AppVidaSana.Data;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.Feeding_Dtos;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWFeedingService : IAWFeeding
    {
        private readonly AppDbContext _bd;

        public AWFeedingService(AppDbContext bd)
        {
            _bd = bd;
        }

        public async Task<List<FeedingsAdminDto>> GetFeedingsAsync(Guid accountID, int page, CancellationToken cancellationToken)
        {
            var feedings = await _bd.UserFeeds
                            .Where(e => e.accountID == accountID)
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);

            var feedingDTOs = feedings.Select(feeding => new FeedingsAdminDto
            {
                userFeedID = feeding.userFeedID,
                userFeedDate = feeding.userFeedDate,
                userFeedTime = feeding.userFeedTime,
                dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                foodsConsumed = [],
                satietyLevel = feeding.satietyLevel,
                emotionsLinked = feeding.emotionsLinked,
                totalCalories = feeding.totalCalories,
                saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl
            }).ToList();

            return feedingDTOs;
        }

        public async Task<List<FeedingsAdminDto>> GetFilterFeedingsAsync(Guid accountID, int page, DateOnly dateInitial, DateOnly dateFinal, CancellationToken cancellationToken)
        {
            var feedings = await _bd.UserFeeds
                            .Where(e => e.accountID == accountID &&
                                        e.userFeedDate >= dateInitial &&
                                        e.userFeedDate <= dateFinal
                            )
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);

            var feedingDTOs = feedings.Select(feeding => new FeedingsAdminDto
            {
                userFeedID = feeding.userFeedID,
                userFeedDate = feeding.userFeedDate,
                userFeedTime = feeding.userFeedTime,
                dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                satietyLevel = feeding.satietyLevel,
                emotionsLinked = feeding.emotionsLinked,
                totalCalories = feeding.totalCalories,
                saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl
            }).ToList();

            return feedingDTOs;
        }

        public async Task<byte[]> ExportAllToCsvAsync(Guid accountID, CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("UserFeedID,UserFeedDate,UserFeedTime,DailyMeal,FoodsConsumed,SatietyLevel,EmotionsLinked,TotalCalories,SaucerPictureUrl");

                while (currentPage >= 0)
                {
                    var feedings = await _bd.UserFeeds
                            .Where(e => e.accountID == accountID)
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .Include(f => f.userFeedNutritionalValues) 
                            .ThenInclude(nv => nv.nutritionalValues) 
                            .ThenInclude(nv => nv.foods)
                            .OrderBy(f => f.userFeedID)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    var feedingDTOs = feedings.Select(feeding => new FeedingsAdminDto
                    {
                        userFeedID = feeding.userFeedID,
                        userFeedDate = feeding.userFeedDate,
                        userFeedTime = feeding.userFeedTime,
                        dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                        foodsConsumed = feeding.userFeedNutritionalValues
                                        .Select(nv => new FoodsConsumedDto
                                        {
                                            foodCode = nv.nutritionalValues.foods.foodCode,
                                            nameFood = nv.nutritionalValues.foods.nameFood,
                                            unit = nv.nutritionalValues.foods.unit,
                                            nutritionalValues = new List<NutritionalValuesDto>
                                            {
                                                new NutritionalValuesDto
                                                {
                                                    nutritionalValueCode = nv.nutritionalValues.nutritionalValueID.ToString(),
                                                    portion = nv.nutritionalValues.portion,
                                                    kilocalories = nv.nutritionalValues.kilocalories,
                                                    protein = nv.nutritionalValues.protein,
                                                    carbohydrates = nv.nutritionalValues.carbohydrates,
                                                    totalLipids = nv.nutritionalValues.totalLipids
                                                }
                                            }
                                        })
                                        .ToList(),
                        satietyLevel = feeding.satietyLevel,
                        emotionsLinked = feeding.emotionsLinked,
                        totalCalories = feeding.totalCalories,
                        saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl
                    }).ToList();

                    if (feedingDTOs.Count == 0)
                    {
                        break;
                    }

                    foreach (var feeding in feedingDTOs)
                    {
                        var foodsConsumedDetails = feeding.foodsConsumed.Select(f =>
                                                    $"(FoodCode: {f.foodCode}, NameFood: {f.nameFood}, Unit: {f.unit}, Portion: {f.nutritionalValues.First().portion}, " +
                                                    $"Kcal: {f.nutritionalValues.First().kilocalories}, Protein: {f.nutritionalValues.First().protein}, " +
                                                    $"Carbohydrates: {f.nutritionalValues.First().carbohydrates}, TotalLipids: {f.nutritionalValues.First().totalLipids})"
                                                ).ToList();

                        var foodsConsumedString = string.Join("; ", foodsConsumedDetails);

                        var csvLine = $"{feeding.userFeedID},{feeding.userFeedDate},{feeding.userFeedTime},{feeding.dailyMeal ?? "N/A"}," +
                                      $"\"{foodsConsumedString}\"," + 
                                      $"{feeding.satietyLevel},\"{feeding.emotionsLinked}\",{feeding.totalCalories},{feeding.saucerPictureUrl ?? "N/A"}";
                        
                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportFilteredToCsvAsync(Guid accountID, DateOnly dateInitial, DateOnly dateFinal, CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("UserFeedID,UserFeedDate,UserFeedTime,DailyMeal,SatietyLevel,EmotionsLinked,TotalCalories,SaucerPictureUrl");

                while (currentPage >= 0)
                {
                    var feedings = await _bd.UserFeeds
                            .Where(e => e.accountID == accountID &&
                                        e.userFeedDate >= dateInitial &&
                                        e.userFeedDate <= dateFinal)
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .OrderBy(f => f.userFeedID)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    var feedingDTOs = feedings.Select(feeding => new FeedingsAdminDto
                    {
                        userFeedID = feeding.userFeedID,
                        userFeedDate = feeding.userFeedDate,
                        userFeedTime = feeding.userFeedTime,
                        dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                        satietyLevel = feeding.satietyLevel,
                        emotionsLinked = feeding.emotionsLinked,
                        totalCalories = feeding.totalCalories,
                        saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl
                    }).ToList();

                    if (feedingDTOs.Count == 0)
                    {
                        break;
                    }

                    foreach (var feeding in feedingDTOs)
                    {
                        var csvLine = $"{feeding.userFeedID},{feeding.userFeedDate},{feeding.userFeedTime},{feeding.dailyMeal ?? "N/A"},{feeding.satietyLevel},\"{feeding.emotionsLinked}\",{feeding.totalCalories},{feeding.saucerPictureUrl ?? "N/A"}";
                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }
    }
}
