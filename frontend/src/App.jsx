import LoginPage from "./components/LoginPage";
import RegisterPage from "./components/RegisterPage";
import Dashboard from "./components/Dashboard";
import AddDevice from "./components/AddDevice";
import { useState } from "react";

export default function App(){

  const [view, setView] = useState(() => localStorage.getItem('authToken') ? 'dashboard' : 'login');
  const [pendingCode, setPendingCode] = useState(() => new URLSearchParams(window.location.search).get('addDevice'));

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('authUsername');
    setView('login');
  };

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