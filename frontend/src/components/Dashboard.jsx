import { useEffect, useState } from 'react';
import { authFetch } from '../api_JWTToken/api';
import './Dashboard.css';

export default function Dashboard({ onLogout }){

    const [summary, setSummary] = useState(null);
    const [error, setError] = useState('');
    const username = localStorage.getItem('authUsername') || 'User';
    const [days, setDays] = useState(1);

    const ApiBackendBase = 'http://localhost:5027/api'
    const token = localStorage.getItem('authToken')

    useEffect(() => {
        async function loadSummary(){
            try{
                const response = await fetch(`${ApiBackendBase}/summary?days=${days}`,{
                    method: "Get",
                    headers: {'Content-Type': 'application/json',
                            'Authorization': `Bearer ${token}`,
                    }
                });
                
                if (response.status === 401) {
                    localStorage.removeItem('authToken');
                    localStorage.removeItem('authUsername');
                    throw new Error('Session expired. Please log in again.');
                }

                if(!response.ok){
                    throw new Error('Could not load the data');
                }
                const data = await response.json();
                console.log(data)
                setSummary(data);
            }catch (err) {
                setError(err.message);
                if (err.message.includes('Session expired')) {
                    onLogout();
                }
            }
        }
        loadSummary();
    }, [token, onLogout, days])

    if (!summary) {
        return <div className="dashboard-container">Loading...</div>;
    }

    const convertTime = (seconds) =>{
        if (!seconds || isNaN(seconds)) return '0:00';

        const hours = Math.floor(seconds/3600);
        const minutes = Math.floor((seconds%3600)/60);
        const formattedMinutes = minutes.toString().padStart(2, '0');

        return `${hours}:${formattedMinutes}`
    } 

    return(
        <div className="dashboard-container">
            
            <div className="header-dashboard">

                <div className='settings-card'>
                   <div className='profile-details'>
                        <div className='profile-img'>
                            <p>{username.charAt(0).toUpperCase()}</p>
                        </div>
                        <p>{username}</p>
                    </div>
                </div>
                
                <div className={`settings-card ${days===1 ? 'active' : ''}`} onClick={() => setDays(1)}>
                    <span>Dashboard</span>
                </div>

                <div className={`settings-card ${days===7 ? 'active' : ''}`} onClick={() => setDays(7)}>
                    <span>Dashboard for 7 days</span>
                </div>

                <div className={`settings-card ${days===30 ? 'active' : ''}`} onClick={() => setDays(30)}>
                    <span>Dashboard for 30 days</span>
                </div>

                <div className='settings-card'>
                    <a  href=''>Predictions</a>
                </div>

                <div className='settings-card'>
                    <button onClick={onLogout} className='btn-logout-dashboard'>Log out</button>
                </div>
            </div>

            <div className='main-eary'>
                <div className='text'>
                    <h2>Welcome Back</h2>
                    <h3>{days === 1 ? "Today's statistics" : `Statistics for the last ${days} days`}</h3>
                </div>

                <div className='row-1'>
                    <div className='block-element'>
                        <p>Whole time</p>
                        <p>{convertTime(summary.totalTimeSpend)}</p>
                    </div>
                    <div className='block-element'>
                        <p>How much app</p>
                        <p>{summary.overall.length}</p>
                    </div>
                    <div className='block-element'>
                        <p>How many devices</p>
                        <p>{summary.byDevice.length}</p>
                    </div>
                </div>

                <div className='row-2'>
                    <div className='app-usage'>
                        <h3 className='card-title'>Top Apps</h3>
                        {summary.overall.length === 0 ? (
                            <p style={{ marginTop: '20px', color: 'var(--lavender-grey)' }}>Started tracking...</p>
                        ) : (
                            summary.overall.map((app, index) => {
                                const percentage = summary.totalTimeSpend > 0 
                                    ? Math.round((app.durationSeconds / summary.totalTimeSpend) * 100) 
                                    : 0;

                                return(<div className='app-row' key={index}>
                                    <span className='app-rank'>{index+1}</span>
                                    <div className='app-info'>
                                        <div className='app-line'>
                                            <span className='app-name'>{app.processName.split('.')[0]}</span>
                                            <span className='app-time'>{convertTime(app.durationSeconds)}</span>
                                        </div>
                                        <div className='bar-track'>
                                            <div className='bar-fill' style={{ width: `${percentage}%`, backgroundColor: 'var(--lavender-grey)' }}></div>
                                        </div>
                                    </div>
                                </div>)
                            }))}
                    </div>
                </div>
            </div>

        </div>
    );
}