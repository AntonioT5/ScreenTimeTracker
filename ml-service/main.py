import uuid
from fastapi import FastAPI, HTTPException
from ResponseAndRequest import *
from datetime import date, timedelta
from predictor import predict_tomorrow
from train_model import train_all_users

app = FastAPI()

@app.get("/health")
def health_check():
    return{"status":"ok"}

@app.post("/predict", response_model=PredictionResponse)
def predict(request: PredicitonRequest):
    try:
        user_id = str(uuid.UUID(request.user_id))
    except ValueError:
        raise HTTPException(status_code=422, detail="user_id must be a UUID")

    if not request.history:
        raise HTTPException(status_code=422, detail="History is empty")

    try:
        prediction_date, total_seconds, top_app = predict_tomorrow(user_id, request.history)
    except FileNotFoundError:
        raise HTTPException(status_code=404, detail="No trained model for this user yet")

    return PredictionResponse(
        prediction_for_date=prediction_date,
        predicted_total_seconds=total_seconds,
        predicted_top_app=top_app
    )

@app.post("/train")
def train():
    train_all_users()
    return {"status": "trained"}