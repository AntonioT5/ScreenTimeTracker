from datetime import datetime
from unittest.mock import patch, MagicMock

from tracker.storage import StoredSession
from tracker.sync import sync_pending_sessions

import requests


def _make_fake_session(session_id: int) -> StoredSession:

    return StoredSession(
        id=session_id,
        process_name="chrome.exe",
        window_title="Test Page",
        start_time=datetime(2026, 9, 11, 10, 0, 0),
        end_time=datetime(2026, 9, 11, 10, 5, 0),
    )

@patch("tracker.sync.mark_synced")
@patch("tracker.sync.get_unsynced_sessions")
@patch("tracker.sync.requests.post")
def test_sync_success_marks_sessions_as_synced(mock_post, mock_get_unsynced, mock_mark_synced):
    fake_sessions = [_make_fake_session(1), _make_fake_session(2)]
    mock_get_unsynced.return_value = fake_sessions

    mock_response = MagicMock()
    mock_response.raise_for_status.return_value = None
    mock_post.return_value = mock_response

    result = sync_pending_sessions()

    assert result == 2
    mock_mark_synced.assert_called_once_with([1, 2])
    mock_post.assert_called_once()


@patch("tracker.sync.mark_synced")
@patch("tracker.sync.get_unsynced_sessions")
@patch("tracker.sync.requests.post")
def test_sync_failure_does_not_mark_sessions_synced(mock_post, mock_get_unsynced, mock_mark_synced):
    mock_get_unsynced.return_value = [_make_fake_session(1)]
    mock_post.side_effect = requests.ConnectionError("simulated network failure")

    result = sync_pending_sessions()

    assert result == 0
    mock_mark_synced.assert_not_called()

@patch("tracker.sync.get_unsynced_sessions")
def test_sync_with_nothing_pending_does_nothing(mock_get_unsynced):
    mock_get_unsynced.return_value = []

    result = sync_pending_sessions()

    assert result == 0
