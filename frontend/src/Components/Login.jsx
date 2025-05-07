import React, { useState } from "react";
import "../css/Login.css";

const Login = () => {
    const [formData, setFormData] = useState({
        email: '',
        password: '',
    });

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await fetch('https://localhost:7083/api/Auth/Login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
            });

            const text = await response.text();
            let data;
            try {
                data = JSON.parse(text);
            } catch {
                data = { message: text };
            }

            if (response.ok) {
                alert('Inloggen gelukt!');
            } else {
                console.error('Inloggen mislukt:', data);
                alert('Er ging iets mis bij inloggen.');
            }
        } catch (error) {
            console.error('Fout bij inloggen:', error);
            alert(error.message);
        }
    };

    return (
        <div className="login-container">
            <h2>Inloggen</h2>
            <form className="login-form" onSubmit={handleSubmit}>
                <input  type="text" name="email" placeholder="Email" value={formData.email} onChange={handleChange} required />
                <input  type="password" name="password" placeholder="Wachtwoord" value={formData.wachtwoord} onChange={handleChange} required />
                <button type="submit" className="login-button">Inloggen</button>
            </form>
        </div>
    );
};

export default Login;
