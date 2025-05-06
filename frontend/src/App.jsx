import React from 'react'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import './App.css'
import Register from './Components/Register'
import Navbar from './Navbar'

function App() {
  return (
    <Router>
      <Navbar />
      <div style={{ padding: '2rem' }}>
        <Routes>
          <Route path="/" element={<div>Home</div>} />
          <Route path="/about" element={<div>Login</div>} />
          <Route path="/Register" element={<Register />} />
        </Routes>
      </div>
    </Router>
  )
}

export default App
