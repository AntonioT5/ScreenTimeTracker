from datetime import date
from pydantic import BaseModel

class AppUsage(BaseModel):
    process_name: str
    duration_seconds: int

class DailyRecord(BaseModel):
    date: date
    apps: list[AppUsage]

class PredicitonRequest(BaseModel):
    user_id: str
    history: list[DailyRecord]

class PredictionResponse(BaseModel):
    prediction_for_date: date
    predicted_total_seconds:int
    predicted_top_app: str