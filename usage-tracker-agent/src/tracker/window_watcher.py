from dataclasses import dataclass
import psutil
import win32gui
import win32process

@dataclass(frozen=True)
class ActiveWindow:
    process_name: str
    window_title: str

def get_active_window() -> ActiveWindow | None:

    hwnd = win32gui.GetForegroundWindow()

    if not hwnd:
        return None

    window_title = win32gui.GetWindowText(hwnd)

    try:
        _, pid = win32process.GetWindowThreadProcessId(hwnd)
        process_name = psutil.Process(pid).name()
    except (psutil.NoSuchProcess, psutil.AccessDenied):
        return None

    return ActiveWindow(process_name=process_name, window_title=window_title)