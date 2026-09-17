import React, {useState} from 'react'
import './RegisterPage.css'

export default function RegisterPage(){

    const [username, setUsername] = useState('');
    const [mail, setMail] = useState('');
    const [password, setPassword] = useState('');
    const [repeatPassword, setRepeatPassword] = useState('');

    const handleSubmit = (event) =>{
        event.preventDeafult()
        console.log(`Registered in with:`, {username, mail, password, repeatPassword})
    }

    return(
        <div className='register-container'>
            {/* Left Page */}
            <div className='register-left'>
                <div className='register-content'>
                    <span className='register-badge'>Your Life Change Today</span>
                    <h1>Take Back Control of Your Time</h1>
                    <p className='register-description'>
                        People often lose more time on screens than they realize mostly on mindless scrolling instead of productive tasks.
                    </p>
                    <p className='register-description'>
                        <strong>Screen Time Tracker</strong> gives you clear statistics across all your devices, tracks app usage and offers predictions to help you invest time into what really matters.
                    </p>
                </div>
            </div>

            {/* RIght Page */}
            <div className='register-rigth'>

                <div className='register-brand-header'>
                    <h2>Screen Time Reader</h2>
                </div>

                <div className='register-form-wrapper'>
                    <h3>Nice to Have You Here</h3>
                    <p className='register-subtitle'>Please enter your details to register</p>

                    <form onSubmit={handleSubmit}>
                        <div className='register-input-group'>
                            <label htmlFor="Username">Username</label>
                            <input 
                            id="Username"
                            placeholder='username'
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                            type="text" />
                        </div>

                        <div className='register-input-group'>
                            <label htmlFor="Mail">Mail</label>
                            <input 
                            id="Mail"
                            placeholder='mail@gmail.com'
                            value={mail}
                            onChange={(e) => setMail(e.target.value)}
                            required
                            type="text" />
                        </div>

                        <div className='register-input-group'>
                            <label htmlFor="Password">Password</label>
                            <input 
                            id="Password"
                            placeholder='password'
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            type="password" />
                        </div>

                        <div className='register-input-group'>
                            <label htmlFor="RPassword">Repeat Password</label>
                            <input 
                            id="RPassword"
                            placeholder='repeat password'
                            value={repeatPassword}
                            onChange={(e) => setRepeatPassword(e.target.value)}
                            required
                            type="password" />
                        </div>

                        <button type='submit' className='register-btn-primary'>
                            Create Account
                        </button>
                    </form>

                    <p className='register-toggle-text'>
                        Already have an account <a href="">Log In now</a>
                    </p>
                </div>
            </div>
        </div>
    );
}