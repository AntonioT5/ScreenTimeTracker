import React, {useState} from 'react'
import './LoginPage.css'

const API_URL = 'http://localhost:5027/api';

export default function LoginPage({ onSwitchToRegister, onSwitchToDashboard }){

    const[username, setUsername] = useState('');
    const[password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (event) => {
        event.preventDefault();
        setError('');
        setLoading(true);

        try{
            const response = await fetch(`${API_URL}/auth/login`,{
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({username, password})
            });

            if(!response.ok){
                throw new Error('Invalid username or password');
            }

            const data = await response.json();
            localStorage.setItem('authToken', data.token);
            localStorage.setItem('authUsername', data.username);

            console.log('Logged in! Token saved.');

            onSwitchToDashboard();
        } catch (err) {
        setError(err.message);
        } finally {
        setLoading(false);
        }
    };
    
    return(
        <div className='container'>
            <div className='auth-container'>
                {/* Left Pannel */}
                <div className='auth-left'>

                    <div className='brand-header'>
                        <h2>Screen Time Reader</h2>
                    </div>

                    <div className='form-wrapper'>
                        <h3>Welcome Back</h3>
                        <p className='subtitle'>Please enter your details to sign in</p>

                        {error && <p className='form-error'>{error}</p>}

                        <form onSubmit={handleSubmit}>
                            <div className='input-group'>
                                <label htmlFor="Username">Username</label>
                                <input 
                                id="Username"
                                placeholder='username'
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                required
                                type="text" />
                            </div>

                            <div className='input-group'>
                                <label htmlFor="Password">Password</label>
                                <input 
                                id="Password"
                                placeholder='password'
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                                type="password" />
                            </div>

                            <button type='submit' className='btn-primary' disabled={loading}>
                                {loading ? 'Signing In...' : 'Sign In'}
                            </button>
                        </form>

                        <p className='toggle-text'>
                            Don't have an account? <a href="#" onClick={(e) => { e.preventDefault(); onSwitchToRegister(); }}>Register now</a>
                        </p>
                    </div>
                </div>

                {/* Right Pannel */}
                <div className='auth-right'>    
                    <div className='overlay-content'>
                        <span className='badge'>Hero of Your Days</span>
                        <h1>Take Back Control of Your Time</h1>
                        <p className='description'>
                            People often lose more time on screens than they realize mostly on mindless scrolling instead of productive tasks.
                        </p>
                        <p className='description'>
                            <strong>Screen Time Tracker</strong> gives you clear statistics across all your devices, tracks app usage and offers predictions to help you invest time into what really matters.
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}