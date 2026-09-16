import logging

import requests
from datetime import timezone

from tracker.config import API_BASE_URL, DEVICE_API_KEY
from tracker.storage import StoredSession, get_unsynced_sessions, mark_synced
from tracker.devices import get_saved_api_key

logger = logging.getLogger(__name__)

def _session_to_payload(session: StoredSession) -> dict:

    return {
        "processName": session.process_name,
        "windowTitle": session.window_title,
        "startTime": session.start_time.astimezone(timezone.utc).isoformat(),
        "endTime": session.end_time.astimezone(timezone.utc).isoformat(),
    }

def sync_pending_sessions() -> int:

    pending = get_unsynced_sessions()
    if not pending:
        return 0

    payload = {"sessions": [_session_to_payload(s) for s in pending]}
    headers = {"Authorization": f"Bearer {get_saved_api_key()}"}

    try:
        response = requests.post(
            f"{API_BASE_URL}/api/sessions/batch",
            json=payload,
            headers=headers,
            timeout=10,
        )
        response.raise_for_status()
    except requests.RequestException:
        logger.warning("Sync failed, will retry next cycle", exc_info=True)
        return 0

    mark_synced([s.id for s in pending])
    return len(pending)