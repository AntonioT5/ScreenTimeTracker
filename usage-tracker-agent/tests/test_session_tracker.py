from datetime import datetime
from unittest.mock import patch

from tracker.session_tracker import SessionTracker
from tracker.window_watcher import ActiveWindow


@patch("tracker.session_tracker.get_active_window")
def test_first_poll_returns_none(mock_get_active_window):

    mock_get_active_window.return_value = ActiveWindow(
        process_name="chrome.exe", window_title="Test"
    )
    tracker = SessionTracker()

    result = tracker.poll_once()

    assert result is None


@patch("tracker.session_tracker.get_active_window")
def test_same_app_returns_none(mock_get_active_window):
    mock_get_active_window.return_value = ActiveWindow(
        process_name="chrome.exe", window_title="Test"
    )

    tracker = SessionTracker()
    result1 = tracker.poll_once()
    result2 = tracker.poll_once()

    assert result1 is None
    assert result2 is None
    


@patch("tracker.session_tracker.get_active_window")
def test_app_change_returns_completed_session(mock_get_active_window):
    window1 = ActiveWindow(
        process_name="chrome.exe",
        window_title="YouTube"
    )

    window2 = ActiveWindow(
        process_name="Code.exe",
        window_title="main.py"
    )

    mock_get_active_window.side_effect = [window1, window2]
    tracker = SessionTracker()
    tracker.poll_once()
    result = tracker.poll_once()

    assert result is not None
    assert result.process_name == window1.process_name