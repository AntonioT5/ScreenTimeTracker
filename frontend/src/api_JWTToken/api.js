const API_URL = 'http://localhost:5027/api';

export async function authFetch(enpoint, options = {}) {
    const token = localStorage.getItem('authToken');

    const response = fetch(`${API_URL}/${enpoint}`, {
        ...options,
        headers:{
            'Content-Type': 'application/json',
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
            ...options.header
        }
    })

    if (response.status === 401) {
        localStorage.removeItem('authToken');
        localStorage.removeItem('authUsername');
        throw new Error('Session expired. Please log in again.');
    }
    
    return response;
}