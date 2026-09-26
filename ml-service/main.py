from fastapi import FastAPI
from ResponseAndRequest import *
from datetime import date, timedelta

app = FastAPI()

@app.get("/health")
def health_check():
    return{"status":"ok"}

@app.post("/predict", response_model=PredictionResponse)
def predict(request: PredicitonRequest):
    tomorrow = date.today() + timedelta(days=1)

    return PredictionResponse(
        prediction_for_date=tomorrow,
        predicted_total_seconds=10800,
        predicted_top_app="chrome.exe"
    )