import React, {useState} from 'react'
import './LoginPage.css'

export default function LoginPage(){

    const[username, setUsername] = useState('');
    const[password, setPassword] = useState('');

    const handleSubmit = (event) => {
        event.preventDefault();
        console.log('Logging in with:', { username, password});
    }
    
    return(

        <div className='auth-container'>
            {/* Left Pannel */}
            <div className='auth-left'>

                <div className='brand-header'>
                    <h2>Screen Time Reader</h2>
                </div>

                <div className='form-wrapper'>
                    <h3>Welcome Back</h3>
                    <p className='subtitle'>Please enter your details to sing in</p>

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

                        <button type='submit' className='btn-primary'>
                            Sign In
                        </button>
                    </form>

                    <p className='toggle-text'>
                        Don't have an account? <a href="">Register now</a>
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

    );
}