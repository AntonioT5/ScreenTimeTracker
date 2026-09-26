using System.Text.Json.Serialization;

namespace Service.DTOs.RequestResponse
{
    public class PredictionAppUsageDto
    {
        [JsonPropertyName("process_name")]
        public string ProcessName { get; set; } = string.Empty;

        [JsonPropertyName("duration_seconds")]
        public int DurationSeconds { get; set; }
    }

    public class DailyRecordDto
    {
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        [JsonPropertyName("apps")]
        public List<PredictionAppUsageDto> Apps { get; set; } = new();
    }

    public class PredictionRequestDto
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("history")]
        public List<DailyRecordDto> History { get; set; } = new();
    }

    public class PredictionResponseDto
    {
        [JsonPropertyName("prediction_for_date")]
        public DateOnly PredictionForDate { get; set; }

        [JsonPropertyName("predicted_total_seconds")]
        public int PredictedTotalSeconds { get; set; }

        [JsonPropertyName("predicted_top_app")]
        public string PredictedTopApp { get; set; } = string.Empty;
    }
}