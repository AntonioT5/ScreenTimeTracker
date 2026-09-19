import LoginPage from "./components/LoginPage";
import RegisterPage from "./components/RegisterPage";
import Dashboard from "./components/Dashboard";
import { useState } from "react";

export default function App(){

  const [view, setView] = useState(() => localStorage.getItem('authToken') ? 'dashboard' : 'login');

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('authUsername');
    setView('login');
  };

  return (<>
        {view === 'login' && (
          <LoginPage onSwitchToRegister={() => setView('register')} onSwitchToDashboard={() => setView('dashboard')}/>
        )}

        {view === 'register' && (
          <RegisterPage onSwitchToLogin={() => setView('login')} />
        )}

        {view === 'dashboard' && (
          <Dashboard onLogout={handleLogout}/>
        )}
  </>);
} 