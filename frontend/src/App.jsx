import React from 'react'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import './App.css'
import Register from './Components/Register'
import Login    from './Components/Login'
import Navbar from './Navbar'
import ProtectedRoute from './Components/ProtectedRoute'
import Home from './Components/Home'

function App() {
  return (
    <Router>
      <Navbar />
      <div>
        <Routes>
          <Route path="/Login"    element={<Login />} />
          <Route path="/Register" element={<Register />} />
          <Route path="/" element={<ProtectedRoute> <Home /> </ProtectedRoute>} />
        </Routes>
      </div>
    </Router>
  )
}       

export default App
