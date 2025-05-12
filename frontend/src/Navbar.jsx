import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './Navbar.css'
import { isAuthenticated } from './auth';

const Navbar = () => {
    const Navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token'); 
        Navigate('/Login');
    };
    
    return (
        <nav className="nav">
            <Link to="/">Home</Link>

            {isAuthenticated() ? (
                <button onClick={handleLogout}>Logout</button>
            ) : (
                <>
                    <Link to="/Login">Login</Link>
                    <Link to="/Register">Register</Link>                
                </>
            )}
        </nav>
    );
};

export default Navbar;
