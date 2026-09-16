import logging
import time

from tracker.session_tracker import SessionTracker
from tracker.storage import save_session
from tracker.sync import sync_pending_sessions
from tracker.config import API_BASE_URL
from tracker.devices import ensure_device_registered

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

SYNC_INTERVAL_SECONDS = 60 

def main():
    api_key = ensure_device_registered()
    tracker = SessionTracker()
    last_sync_time = time.monotonic()

    logger.info("Usage tracker agent started")

    while True:
        session = tracker.poll_once()
        if session is not None:
            save_session(session)
            logger.info(f"Logged session: {session.process_name} ({session.duration_seconds:.1f}s)")

        now = time.monotonic()

        if now - last_sync_time >= SYNC_INTERVAL_SECONDS:
            synced_count = sync_pending_sessions()
            if synced_count:
                logger.info(f"Synced {synced_count} sessions")
            last_sync_time = now

        time.sleep(tracker.poll_interval_seconds)

if __name__ == "__main__":
    main()