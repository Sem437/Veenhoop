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

    const getRoles = () => {
    const token = localStorage.getItem('token');
        if (token) {
            try {
                const decoded = jwtDecode(token);
                const rollen = decoded?.role || decoded?.Rol || decoded?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
                return Array.isArray(rollen) ? rollen : [rollen];
            } catch (error) {
                console.error("Error decoding token:", error);
                return [];
            }
            }
            return [];
    };


    return (
        <nav className="nav">
            <div className="nav-links">
                <Link to="/">Home</Link>
                
                {isAuthenticated() && getRoles().includes('Docent') && (
                    <>
                        <Link to="/Overzicht">Overzicht</Link>
                        <Link to="/Klassen">Klassen</Link>
                        <Link to="/Toetsen">Toetsen</Link>
                    </>

                )}

                {isAuthenticated() && getRoles().includes('Administrator') && (
                    <>
                        <Link to="/Rollen">Rollen</Link> 
                        <Link to="/Docentaanmaken">Docent</Link>                       
                    </>
                )}
            </div>            
            <div className="nav-rechts">
                {isAuthenticated() ? (
                    <button onClick={handleLogout} className='logout-button'>Log uit</button>
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