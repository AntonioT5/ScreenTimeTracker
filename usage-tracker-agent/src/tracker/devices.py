import json
import logging
import platform
import secrets
import socket
import time
import webbrowser
from pathlib import Path

import requests

from tracker.config import API_BASE_URL, DASHBOARD_URL

logger = logging.getLogger(__name__)

POLL_SECONDS = 2
TIMEOUT_SECONDS = 600

PROJECT_ROOT = Path(__file__).resolve().parent.parent.parent

DEVICE_CONFIG_PATH = PROJECT_ROOT / "usage_tracker" / "device.json"


def get_saved_api_key() -> str | None:
    if not DEVICE_CONFIG_PATH.exists():
        return None
    data = json.loads(DEVICE_CONFIG_PATH.read_text())
    return data.get("api_key")


def _save_api_key(api_key: str) -> None:
    DEVICE_CONFIG_PATH.parent.mkdir(parents=True, exist_ok=True)
    DEVICE_CONFIG_PATH.write_text(json.dumps({"api_key": api_key}))


def ensure_device_registered() -> str:
    existing_key = get_saved_api_key()
    if existing_key:
        return existing_key

    code = secrets.token_urlsafe(16)

    try:
        requests.post(
            f"{API_BASE_URL}/api/devices/pending",
            json={
                "code": code,
                "deviceName": socket.gethostname(),
                "operatingSystem": platform.system(),
            },
            timeout=10,
        ).raise_for_status()
    except requests.RequestException as error:
        raise SystemExit(f"Could not reach the backend: {error}")

    link = f"{DASHBOARD_URL}/?addDevice={code}"
    webbrowser.open(link)

    deadline = time.monotonic() + TIMEOUT_SECONDS
    while time.monotonic() < deadline:
        try:
            response = requests.get(f"{API_BASE_URL}/api/devices/pending/{code}", timeout=10)
        except requests.RequestException:
            time.sleep(POLL_SECONDS)
            continue

        if response.status_code == 200:
            api_key = response.json()["apiKey"]
            _save_api_key(api_key)
            print("Device added. Tracking started.")
            return api_key
        if response.status_code == 404:
            raise SystemExit("The setup code expired. Run again.")

        time.sleep(POLL_SECONDS)

    raise SystemExit("Timed out waiting for confirmation.")