import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './Navbar.css';
import { isAuthenticated } from './auth'; 
import { jwtDecode } from 'jwt-decode';

const Navbar = () => {
    const navigate = useNavigate(); // Corrected to lowercase

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/Login');
    };

    const getRole = () => {
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const decoded = jwtDecode(token);
                // Check common role claim names
                const role = decoded?.role || decoded?.Rol || decoded?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
                return role || null;
            } catch (error) {
                console.error("Error decoding token:", error);
                return null; // Return null if token is invalid or decoding fails
            }
        }
        return null;
    };

    return (
        <nav className="nav">
            <div className="nav-links">
                <Link to="/">Home</Link>
                
                {isAuthenticated() && getRole() === 'Docent' && (
                    <>
                        <Link to="/Overzicht">Overzicht</Link>
                        <Link to="/Klassen">Klassen</Link>
                    </>

                )}
            </div>            
            <div className="nav-rechts">
                {isAuthenticated() ? (
                    <button onClick={handleLogout}>Logout</button>
                ) : (
                    <>
                        <Link to="/Login">Login</Link>
                        <Link to="/Register">Register</Link>
                    </>
                )}
            </div>
        </nav>
    );
};

export default Navbar;