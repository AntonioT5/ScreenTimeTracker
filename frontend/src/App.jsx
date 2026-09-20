import LoginPage from "./components/LoginPage";
import RegisterPage from "./components/RegisterPage";
import Dashboard from "./components/Dashboard";
import AddDevice from "./components/AddDevice";
import { useCallback, useState } from "react";

const ApiBackendBase = 'http://localhost:5027/api';

export default function App(){

  const [view, setView] = useState(() => localStorage.getItem('authToken') ? 'dashboard' : 'login');
  const [pendingCode, setPendingCode] = useState(() => new URLSearchParams(window.location.search).get('addDevice'));

  const handleLogout = useCallback(async () => {
    const token = localStorage.getItem('authToken');
    const deviceName = localStorage.getItem('deviceName');

    await fetch(`${ApiBackendBase}/devices/unlink`, {
      method: 'POST',
      headers: {
        'Content-Type': `application/json`,
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({
        'DeviceName': deviceName,
      })
    })

    localStorage.removeItem('authToken');
    localStorage.removeItem('authUsername');
    localStorage.removeItem('deviceName')
    setView('login');
  }, []);

  const finishAddDevice = () => {
    setPendingCode(null);
    window.history.replaceState({}, '', window.location.pathname);
  };

  return (<>
        {view === 'login' && (
          <LoginPage onSwitchToRegister={() => setView('register')} onSwitchToDashboard={() => setView('dashboard')}/>
        )}

        {view === 'register' && (
          <RegisterPage onSwitchToLogin={() => setView('login')} />
        )}

        {view === 'dashboard' && pendingCode && (
          <AddDevice code={pendingCode} onDone={finishAddDevice} onLogout={handleLogout}/>
        )}

        {view === 'dashboard' && !pendingCode && (
          <Dashboard onLogout={handleLogout}/>
        )}
  </>);
}