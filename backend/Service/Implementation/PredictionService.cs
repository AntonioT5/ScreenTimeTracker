using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;
using System.Net.Http;
using Service.DTOs.RequestResponse;
using System.Net.Http.Json;

namespace Service.Implementation
{
    public class PredictionService : IPredictionService
    {
        private readonly IRepository<DailySummary> _summaryRepository;
        private readonly IRepository<Prediction> _predictionRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PredictionService> _logger;
        public PredictionService(IRepository<DailySummary> summaryRepository,IRepository<Prediction> predictionRepository,IHttpClientFactory httpClientFactory,ILogger<PredictionService> logger)
        {
            _summaryRepository = summaryRepository;
            _predictionRepository = predictionRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public async Task GeneratePredictionAsync(Guid userId)
        {
            var since = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-7));

            var rows = await _summaryRepository.GetAllAsync(
                selector: s => new {s.Date, s.ProcessName, s.DurationSeconds},
                predicate: s=> s.UserId==userId && s.Date>= since
            );

            var rowList = rows.ToList();

            if(rowList.Count == 0)
            {
                _logger.LogInformation("No history for user {userid}, skipping prediciton", userId);
                return;
            }
            var history = rowList.GroupBy(r=>r.Date)
                .Select(g=> new DailyRecordDto
                {
                    Date = g.Key,
                    Apps = g.Select(x => new PredictionAppUsageDto
                    {
                        ProcessName = x.ProcessName,
                        DurationSeconds = x.DurationSeconds
                    }).ToList()
                })
                .OrderBy(d => d.Date)
                .ToList();

            var request = new PredictionRequestDto
            {
                UserId = userId.ToString(),
                History = history
            };

            var client = _httpClientFactory.CreateClient("MlService");
            var httpResponse = await client.PostAsJsonAsync("/predict", request);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorBody = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogError("Prediction request failed for {UserId}: {Status} - {Body}", userId, httpResponse.StatusCode, errorBody);
                return;
            }

            var result = await httpResponse.Content.ReadFromJsonAsync<PredictionResponseDto>();
            if (result is null) return;

            var alreadyExists = await _predictionRepository.Get(
                selector: p => p.Id,
                predicate: p => p.UserId == userId && p.PredictionForDate == result.PredictionForDate);

            if (alreadyExists != Guid.Empty)
            {
                _logger.LogInformation("Prediction already exists for {UserId} on {Date}", userId, result.PredictionForDate);
                return;
            }

            await _predictionRepository.InsertAsync(new Prediction
            {
                UserId = userId,
                PredictionForDate = result.PredictionForDate,
                PredictedTotalSeconds = result.PredictedTotalSeconds,
                PredictedTopApp = result.PredictedTopApp,
                GeneratedAt = DateTime.UtcNow
            });

            _logger.LogInformation("Saved prediction for {UserId} on {Date}", userId, result.PredictionForDate);
        }
    }
}