from sklearn.linear_model import LinearRegression, LogisticRegression

from features import build_training_data
import joblib
from pathlib import Path
import os
import pandas as pd
from dotenv import load_dotenv
from sqlalchemy import create_engine

from features import prepare_user_features


MODELS_DIR = Path(__file__).parent / "models"


def save_models(user_id, regressor, classifier):
    MODELS_DIR.mkdir(exist_ok=True)
    joblib.dump(regressor, MODELS_DIR / f"{user_id}_regressor.joblib")
    joblib.dump(classifier, MODELS_DIR / f"{user_id}_classifier.joblib")


def load_models(user_id):
    regressor = joblib.load(MODELS_DIR / f"{user_id}_regressor.joblib")
    classifier = joblib.load(MODELS_DIR / f"{user_id}_classifier.joblib")
    return regressor, classifier

def train_models(feature_df):
    X, y_total, y_top_app = build_training_data(feature_df)

    regressor = LinearRegression()
    regressor.fit(X, y_total)

    classifier = LogisticRegression(max_iter=1000)
    classifier.fit(X, y_top_app)

    return regressor, classifier

MIN_TRAINING_ROWS = 7

def load_all_data():
    load_dotenv()
    engine = create_engine(
        f"postgresql+psycopg2://{os.getenv('DB_USER')}:{os.getenv('DB_PASSWORD')}"
        f"@{os.getenv('DB_HOST')}:{os.getenv('DB_PORT')}/{os.getenv('DB_NAME')}"
    )
    query = """
    SELECT "UserId", "Date", "ProcessName", "DurationSeconds"
    FROM "DailySummaries"
    ORDER BY "Date"
    """
    return pd.read_sql(query, engine)


def train_all_users():
    raw_df = load_all_data()

    for user_id, user_df in raw_df.groupby("UserId"):
        filled_df, feature_df = prepare_user_features(user_df)
        X, y_total, y_top_app = build_training_data(feature_df)

        if len(X) < MIN_TRAINING_ROWS or y_top_app.nunique() < 2:
            print(f"Skipping {user_id}: not enough data yet ({len(X)} rows)")
            continue

        regressor, classifier = train_models(feature_df)
        save_models(user_id, regressor, classifier)
        print(f"Trained and saved models for {user_id} ({len(X)} rows)")

if __name__ == "__main__":
    train_all_users()
    