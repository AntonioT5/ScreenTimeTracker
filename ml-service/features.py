import pandas as pd
from datetime import timedelta

def build_daily_dataframe(history: list) -> pd.DataFrame:
    rows = []
    for day in history:
        total = sum(app.duration_seconds for app in day.apps)
        top_app = max(day.apps, key=lambda x: x.duration_seconds).process_name if day.apps else None
        rows.append({"date": day.date, "total_seconds": total, "top_app": top_app})

    df = pd.DataFrame(rows)
    df = df.sort_values("date").reset_index(drop=True)
    return df

def add_features(df: pd.DataFrame) -> pd.DataFrame:
    df = df.copy()
    df["day_of_week"] = pd.to_datetime(df["date"]).dt.dayofweek
    df["is_weekend"] = df["day_of_week"].isin([5, 6]).astype(int)
    df["previous_day_total"] = df["total_seconds"].shift(1)
    df["rolling_7day_avg"] = df["total_seconds"].shift(1).rolling(window=7, min_periods=1).mean()

    df = pd.get_dummies(df, columns=["day_of_week"], prefix="dow", drop_first=True)
    
    return df

def fill_missing_days(df: pd.DataFrame) -> pd.DataFrame:
    df = df.copy()
    df["date"] = pd.to_datetime(df["date"])
    df = df.set_index("date")

    full_range = pd.date_range(start=df.index.min(), end=df.index.max(), freq="D")
    df = df.reindex(full_range)

    df["total_seconds"] = df["total_seconds"].fillna(0)
    df["top_app"] = df["top_app"].fillna("none")

    df.index.name = "date"
    df = df.reset_index()
    return df

def build_prediction_features(df: pd.DataFrame):
    last_date = df["date"].max()
    tomorrow = last_date + timedelta(days=1)

    tomorrow_day_of_week = pd.Timestamp(tomorrow).dayofweek
    previous_day_total = df.iloc[-1]["total_seconds"]
    rolling_7day_avg = df["total_seconds"].tail(7).mean()

    row = {
        "previous_day_total": previous_day_total,
        "rolling_7day_avg": rolling_7day_avg
    }
    for i in range(1, 7):
        row[f"dow_{i}"] = 1 if i == tomorrow_day_of_week else 0

    features = pd.DataFrame([row])
    return features, tomorrow