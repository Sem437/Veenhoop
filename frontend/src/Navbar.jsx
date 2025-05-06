import React from 'react';
import { Link } from 'react-router-dom';
import './Navbar.css'

export default function Navbar() {
    return (
        <nav className="nav">
            <a href="/">Home</a>
            <a href="/Login">Login</a>
            <a href="/Register">Register</a>
        </nav>
    )
}