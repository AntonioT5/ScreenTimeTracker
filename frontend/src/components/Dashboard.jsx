import { useEffect, useState } from 'react';
import { authFetch } from '../api_JWTToken/api';
import './Dashboard.css';

export default function Dashboard({ onLogout }){

    const [summary, setSummary] = useState(null);
    const [error, setError] = useState('');
    const username = localStorage.getItem('authUsername') || 'User';

    useEffect(() => {
        async function loadSummary(){
            try{
                const response = await authFetch('/summary');
                if(!response.ok){
                    throw new Error('Could not load the data');
                }
                const data = await response.json();
                setSummary(data);
            }catch (err) {
                setError(err.message);
                if (err.message.includes('Session expired')) {
                    onLogout();
                }
            }
        }
        loadSummary();
    }, [])

    return(
        <div className="dashboard-container">
            
            <div className="header-dashboard">
                <div className="left-part">
                    <div className="profile-picture">
                        <p>{username.charAt(0).toUpperCase()}</p>
                    </div>
                    <div className="profile-name">
                        {username}
                    </div>
                </div>

                <div className="center-part">
                    <a href="">Dashboard</a>
                    <a href="">Devices</a>
                    <a href="">Settings</a>
                </div>

                <div className="right-part">
                    <button className="btn-logout" onClick={onLogout}>Log Out</button>
                </div>
            </div>

            <div className="main-dashboard">

                <div className="text-up">
                    Here's your screen time overview for today
                </div>

                {error && <p className="form-error">{error}</p>}

                <div className="">

                </div>

            </div>

        </div>
    );
}