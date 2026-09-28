import { useEffect, useState } from 'react';
import './Dashboard.css';

export default function Dashboard({ onLogout, onEditChanges }){

    const [summary, setSummary] = useState(null);
    const [error, setError] = useState('');

    const username = localStorage.getItem('authUsername') || 'User';
    const mail = localStorage.getItem('authMail')
    const [editUsername, setEditUsername] = useState(username);
    const [editMail, setEditMail] = useState(mail);
    const [prediction, setPrediction] = useState(null);

    const [days, setDays] = useState(1);
    const [logoutFlag, setLogoutFlag] = useState(false);
    const [deviceFlag, setDeviceFlag] = useState(false);
    const [userModel, setUserModel] = useState(false);
    const [predictionFlag, setPredictionFlag] = useState("notClicked");
    const [editError, setEditError] = useState('');
    const [activeMenu, setActiveMenu] = useState("dashboard");

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
                    localStorage.removeItem('authMail');
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

    const loadPrediciton = async () => {
        setPredictionFlag("waiting")

        try{
            const response = await fetch(`${ApiBackendBase}/prediction/generate`, {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });

            if(!response.ok){
                throw new Error('You need at least 7 days for prediction. Try again latter.');
            }
            const data = await response.json();
            setPrediction(data);
            setPredictionFlag("clicked")
        }catch (err) {
            console.error(err);
            setPredictionFlag("error")
            setError(err.message);
        }
    }


    return(
        <div className="dashboard-container">
            
            <div className="header-dashboard">

                <div className='settings-card'>
                   <div className='profile-details'>
                        <div className='profile-img'>
                            <p>{username.charAt(0).toUpperCase()}</p>
                        </div>
                        <p onClick={() => setUserModel(true)}>{username}</p>
                    </div>
                </div>
                
                <div className={`settings-card ${activeMenu === "dashboard" ? "active" : ""}`} onClick={() => {setActiveMenu("dashboard"); setDays(1);}}>
                    <span>Dashboard</span>
                </div>

                <div className={`settings-card ${activeMenu === "7days" ? "active" : ""}`} onClick={() => {setActiveMenu("7days"); setDays(7);}}>
                    <span>Dashboard for 7 days</span>
                </div>

                <div className={`settings-card ${activeMenu === "30days" ? "active" : ""}`} onClick={() => {setActiveMenu("30days"); setDays(30);}}>
                    <span>Dashboard for 30 days</span>
                </div>

                <div className={`settings-card ${activeMenu === "prediction" ? "active" : ""}`} onClick={()=>{setActiveMenu("prediction"); loadPrediciton();}}>
                    <span>Predictions</span>
                </div>

                <div className='settings-card'>
                    <button onClick={() => {setLogoutFlag(true)}} className='btn-logout-dashboard'>Log out</button>
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
                    <div className='block-element clickable' onClick={() => setDeviceFlag(true)}>
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
            
            <div style={{display: logoutFlag ? 'flex' : 'none'}}  className='model' onClick={() => setLogoutFlag(false)}>
                <div onClick={(e) => e.stopPropagation()}>
                    <p>Are You Sure?</p>
                    <div className='buttons-area'>
                        <button onClick={onLogout} className='btn-logout-dashboard'>Yes</button>
                        <button onClick={() => {setLogoutFlag(false)}} className='btn-logout-dashboard'>No</button>
                    </div>
                </div>
            </div>

            <div style={{display: predictionFlag === "notClicked" ? 'none' : 'flex'}}  className='model-prediction' onClick={() => setPredictionFlag("notClicked")}>
                <div onClick={(e) => e.stopPropagation()}>
                    <div>
                        <p className='p1'>Tomorrow's Predictions</p>

                        {predictionFlag === "waiting" && <p className='loading'>Loading data...</p>} 
                        {predictionFlag === "error" && <p className='loading'>Something went worng. Try again latter</p>} 
                        {predictionFlag === "clicked" && 
                            <div className='prediction-container'>
                                {prediction.mostUsedApp !== "none" ?
                                    <><div className='prediction-element'>
                                        <p>Screen time</p>
                                        <p>{convertTime(prediction.totalScreenTime)}</p>
                                    </div>
                                    <div className='prediction-element'>
                                        <p>Top app</p>
                                        <p>{prediction.mostUsedApp}</p>
                                    </div></> :
                                        <>
                                            <div className='result-predicition'>
                                                <p>Tomorrow is you rest day. 0 Screen time</p>
                                            </div>
                                        </>
                                }     
                            </div>                
                        } 

                        <div className='buttons-area'>
                            <button onClick={() => {setPredictionFlag("notClicked"); setActiveMenu("dashboard"); setDays(1);}} className='btn-logout-dashboard btn-large'>Close</button>
                        </div>
                        <div>
                            <p className='waring-prediction'>This is only a prediciton and may not be accurate.</p>
                        </div>
                    </div>
                </div>
            </div>

            <div style={{display: userModel ? 'flex' : 'none'}}  className='model-user' onClick={() => setUserModel(false)}>
                <div onClick={(e) => e.stopPropagation()}>
                    <p>Edit Your Account</p>
                    {editError && <p className='form-error'>{editError}</p>}
                    <div className='edit-model'>
                        <div className='edit-inputs'>
                            <p>Username</p>
                            <input type="text" value={editUsername} onChange={(e) => setEditUsername(e.target.value)} />
                        </div>
                        <div className='edit-inputs'>
                            <p>Mail</p>
                            <input type="email" value={editMail} onChange={(e) => setEditMail(e.target.value)}/>
                        </div>
                            
                    </div>
                    <div className='buttons-area'>
                        <button onClick={async () => {
                            setEditError('');
                            try {
                                await onEditChanges(editUsername, editMail);
                                setUserModel(false);
                            } catch (err) {
                                setEditError(err.message);
                                setEditMail(mail);
                                setEditUsername(username);
                            }
                            }}
                            className='btn-logout-dashboard btn-large'>Save Changes</button>
                        <button onClick={() => {setUserModel(false)}} className='btn-logout-dashboard btn-large'>Close</button>
                    </div>
                </div>
            </div>

            <div style={{display: deviceFlag ? 'flex' : 'none'}} className='model-device' onClick={() => setDeviceFlag(false)}>
                <div onClick={(e) => e.stopPropagation()}>
                    {summary.overall.length === 0 ? (
                        <p>
                            You didn't add a device
                        </p>
                    ) : (
                        <>
                            <p>Your Added Devices</p>

                            {summary.byDevice.map((d, i) => {

                                const timeSpent = d.apps.reduce((sum, app) => {
                                    return sum + app.durationSeconds;
                                }, 0);

                                return (
                                    <div className='device-name-layer' key={i}>
                                        <p>{d.deviceName}</p>
                                        <p>{convertTime(timeSpent)}</p>
                                    </div>
                                );
                            })}
                        </>
                    )}

                    <div>
                        <button
                            onClick={() => setDeviceFlag(false)} className='btn-logout-dashboard'>
                            Close
                        </button>
                    </div>
                </div>
            </div>

        </div>
    );
}