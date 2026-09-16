import getpass
import json
import logging
import platform
import socket
from pathlib import Path

import requests

from tracker.config import API_BASE_URL

logger = logging.getLogger(__name__)

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

    print("First-time setup: this device needs to be registered.")
    username = input("Username: ")
    password = getpass.getpass("Password: ")

    response = requests.post(
        f"{API_BASE_URL}/api/devices",
        json={
            "username": username,
            "password": password,
            "deviceName": socket.gethostname(),
            "operatingSystem": platform.system(),
        },
        timeout=10,
    );
    response.raise_for_status()

    api_key = response.json()["apiKey"]
    _save_api_key(api_key)
    print("Device registered successfully.")
    return api_key

    