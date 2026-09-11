import time
from dataclasses import dataclass
from datetime import datetime

from tracker.window_watcher import ActiveWindow, get_active_window

@dataclass(frozen=True)
class Session:
    process_name: str
    window_title: str
    start_time: datetime
    end_time: datetime

    @property
    def duration_seconds(self) -> float:
        return (self.end_time - self.start_time).total_seconds()

class SessionTracker:

    def __init__(self, poll_interval_seconds: float=1.5):
        self.poll_interval_seconds = poll_interval_seconds
        self._current: ActiveWindow | None = None
        self._current_start: datetime | None = None

    def poll_once(self) ->Session | None:
        active = get_active_window()
        now = datetime.now()

        if active is None:
            return None

        if self._current is None:
            self._current = active
            self._current_start = now
            return None

        if active.process_name == self._current.process_name: #same app don't save
            return None

        completed = Session(
            process_name = self._current.process_name,
            window_title = self._current.window_title,
            start_time = self._current_start,
            end_time=now
        )

        self._current = active
        self._current_start = now
        return completed

    def run_forever(self, on_session_complete):

        while True:
            session = self.poll_once()
            if session is not None:
                on_session_complete(session)
            time.sleep(self.poll_interval_seconds)
    