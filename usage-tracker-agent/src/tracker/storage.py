import sqlite3
from dataclasses import dataclass
from datetime import datetime
from pathlib import Path

from tracker.session_tracker import Session

PROJECT_ROOT = Path(__file__).resolve().parent.parent.parent

DB_PATH = PROJECT_ROOT / "data" / "sessions.db"

def _get_connection() -> sqlite3.Connection:
    DB_PATH.parent.mkdir(parents=True, exist_ok=True)
    conn = sqlite3.connect(DB_PATH)
    conn.execute("""
                CREATE TABLE IF NOT EXISTS sessions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    process_name TEXT NOT NULL,
                    window_title TEXT NOT NULL,
                    start_time TEXT NOT NULL,
                    end_time TEXT NOT NULL,
                    synced INTEGER NOT NULL DEFAULT 0
                )
    """)
    return conn

def save_session(session: Session) -> None:
    conn = _get_connection()

    try:
        conn.execute("""
                    INSERT INTO sessions (process_name, window_title, start_time, end_time, synced)
                    VALUES (?, ?, ?, ?, 0)
                    """, 
                    (
                        session.process_name,
                        session.window_title,
                        session.start_time.isoformat(),
                        session.end_time.isoformat(),
                    )
        )
        conn.commit()
    finally:
        conn.close()

@dataclass(frozen=True)
class StoredSession:
    id: int
    process_name: str
    window_title: str
    start_time: datetime
    end_time: datetime

def get_unsynced_sessions() -> list[StoredSession]:

    conn = _get_connection()

    try:
        rows = conn.execute("""
                            "SELECT id, process_name, window_title, start_time, end_time"
                            "FROM sessions WHERE synced=0 ORDER BY start_time"
        """).fetchall()
        
        return [StoredSession(
                id=row[0],
                process_name=row[1],
                window_title=row[2],
                start_time=datetime.fromisoformat(row[3]),
                end_time=datetime.fromisoformat(row[4]),
            ) for row in rows]
        
    finally:
            conn.close()

def mark_synced(session_ids: list[int]) -> None:
     
    if not session_ids:
        return

    conn = _get_connection()
    try:
        placeholder = ",".join("?" * len(session_ids))
        conn.execute(f"UPDATE sessions SET synced=1 WHERE id IN ({placeholder})", (session_ids))
        conn.commit()
    finally:
        conn.close()