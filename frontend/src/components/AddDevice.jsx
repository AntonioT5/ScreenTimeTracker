import { useEffect, useState } from 'react';
import './AddDevice.css';

const ApiBackendBase = 'http://localhost:5027/api';

export default function AddDevice({ code, onDone, onLogout }) {
    const [info, setInfo] = useState(null);
    const [status, setStatus] = useState('loading');
    const token = localStorage.getItem('authToken');
    const username = localStorage.getItem('authUsername') || 'User';

    useEffect(() => {
        async function loadInfo() {
            try {
                const response = await fetch(`${ApiBackendBase}/devices/pending/${code}/info`, {
                    headers: { Authorization: `Bearer ${token}` },
                });

                if (response.status === 401) {
                    onLogout();
                    return;
                }
                if (!response.ok) {
                    setStatus('error');
                    return;
                }

                setInfo(await response.json());
                setStatus('ready');
            } catch {
                setStatus('error');
            }
        }

        loadInfo();
    }, [code, token, onLogout]);

    async function handleYes() {
        setStatus('sending');
        try {
            const response = await fetch(`${ApiBackendBase}/devices/claim`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ code }),
            });

            if (response.status === 401) {
                onLogout();
                return;
            }
            setStatus(response.ok ? 'done' : 'error');
        } catch {
            setStatus('error');
        }
    }

    return (
        <div className="add-device">
            <div className="add-device-card">
                {status === 'loading' && <p>Loading...</p>}

                {(status === 'ready' || status === 'sending') && info && (
                    <>
                        <h2>Add this device?</h2>
                        <p className="add-device-name">{info.deviceName} ({info.operatingSystem})</p>
                        <p className="add-device-hint">Adding to account: <b>{username}</b></p>
                        <div className="add-device-buttons">
                            <button onClick={handleYes} disabled={status === 'sending'}>
                                {status === 'sending' ? 'Adding...' : 'Yes'}
                            </button>
                            <button onClick={onDone} disabled={status === 'sending'}>No</button>
                        </div>
                        <button className="add-device-switch" onClick={onLogout}>
                            Not you? Log out
                        </button>
                    </>
                )}

                {status === 'done' && (
                    <>
                        <h2>Device added</h2>
                        <p className="add-device-hint">Tracking starts in a few seconds.</p>
                        <button onClick={onDone}>Go to dashboard</button>
                    </>
                )}

                {status === 'error' && (
                    <>
                        <h2>Something went wrong</h2>
                        <p className="add-device-hint">This link is invalid or expired.</p>
                        <button onClick={onDone}>Go to dashboard</button>
                    </>
                )}
            </div>
        </div>
    );
}