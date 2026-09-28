from features import build_daily_dataframe, fill_missing_days, build_prediction_features
from train_model import load_models

def predict_tomorrow(user_id, history):
    regressor, classifier = load_models(user_id)

    daily_df = build_daily_dataframe(history)
    filled_df = fill_missing_days(daily_df)
    pred_features, tomorrow = build_prediction_features(filled_df)

    total_seconds = int(max(0, regressor.predict(pred_features)[0]))
    top_app = str(classifier.predict(pred_features)[0])

    return tomorrow.date(), total_seconds, top_app