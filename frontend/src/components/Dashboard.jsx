import { useEffect, useState } from 'react';
import { authFetch } from '../api_JWTToken/api';
import './Dashboard.css';

export default function Dashboard({ onLogout }){

    const [summary, setSummary] = useState(null);
    const [error, setError] = useState('');
    const username = localStorage.getItem('authUsername') || 'User';

    // useEffect(() => {
    //     async function loadSummary(){
    //         try{
    //             const response = await authFetch('/summary');
    //             if(!response.ok){
    //                 throw new Error('Could not load the data');
    //             }
    //             const data = await response.json();
    //             setSummary(data);
    //         }catch (err) {
    //             setError(err.message);
    //             if (err.message.includes('Session expired')) {
    //                 onLogout();
    //             }
    //         }
    //     }
    //     loadSummary();
    // }, [])

    return(
        <div className="dashboard-container">
            
            <div className="header-dashboard">

                <div className='settings-card'>
                   <div className='profile-details'>
                        <div className='profile-img'>
                            <p>A</p>
                        </div>
                        <p>Antonio</p>
                    </div>
                </div>
                
                <div className='settings-card active'>
                    <a href=''>Dashboard</a>
                </div>

                <div className='settings-card'>
                    <a  href=''>Statistics for 7 days</a>
                </div>

                <div className='settings-card'>
                    <a  href=''>Statistics for 30 days</a>
                </div>

                <div className='settings-card'>
                    <a  href=''>Predictions</a>
                </div>
            </div>

            <div className='main-eary'>
                <div className='text'>
                    <h2>Welcome Back</h2>
                    <h3>Today's statistics</h3>
                </div>

                <div className='row-1'>
                    <div className='block-element'>
                        <p>Whole time</p>
                        <p>2:54</p>
                    </div>
                    <div className='block-element'>
                        <p>How much app:</p>
                        <p>12</p>
                    </div>
                    <div className='block-element'>
                        <p>How many devices</p>
                        <p>2</p>
                    </div>
                </div>

                <div className='row-2'>
                    <div className='app-usage'>
                        <h3 className='card-title'>Top 5 apps</h3>
                        <div className='app-row'>
                            <span className='app-rank'>1</span>
                            <div className='app-info'>
                                <div className='app-line'>
                                    <span className='app-name'>Choreme</span>
                                    <span className='app-time'>2:55</span>
                                </div>
                                <div className='bar-track'>
                                    <div className='bar-fill' style={{ width: `68%`, backgroundColor: 'yellow' }}></div>
                                </div>
                            </div>
                        </div>
                        
                        <div className='app-row'>
                            <span className='app-rank'>2</span>
                            <div className='app-info'>
                                <div className='app-line'>
                                    <span className='app-name'>Choreme</span>
                                    <span className='app-time'>2:55</span>
                                </div>
                                <div className='bar-track'>
                                    <div className='bar-fill' style={{ width: `68%`, backgroundColor: 'yellow' }}></div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

        </div>
    );
}