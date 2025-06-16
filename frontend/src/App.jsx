import React, { useState, useEffect } from 'react'
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom'

import './App.css'
import Register from './Components/Register'
import Login from './Components/Login'
import Navbar from './Navbar'
import ProtectedRoute from './Components/ProtectedRoute'
import Home from './Components/Home'
import DocentHome from './Components/Docenten/DocentHome'
import Klas from './Components/Docenten/Klas'
import CijferWijzigen from './Components/Docenten/CijferWijzigen'
import DocentenOverzicht from './Components/Docenten/DocentenOverzicht'
import DocentKlassen from './Components/Docenten/Klassen'
import DocentKlassenWijzigen from './Components/Docenten/KlassenWijzigen'

// 👇 functie om payload van JWT te decoden zonder externe lib
function parseJwt(token) {
  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => `%${('00' + c.charCodeAt(0).toString(16)).slice(-2)}`)
        .join('')
    )
    return JSON.parse(jsonPayload)
  } catch (e) {
    return null
  }
}

function App() {
  const [role, setRole] = useState(null)
  const [tokenChecked, setTokenChecked] = useState(false)

  useEffect(() => {
    const token = localStorage.getItem('token')
    if (!token) {
      setRole(null)
      setTokenChecked(true)
      return
    }

    const decoded = parseJwt(token)
    if (decoded) {
      const rol =
        decoded.role ||
        decoded.Rol ||
        decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      setRole(rol)
    } else {
      console.error('Token is ongeldig of niet te parsen.')
      setRole(null)
    }

    setTokenChecked(true)
  }, [])

  if (!tokenChecked) {
    return <div>Loading...</div>
  }

  return (
    <Router>
      <Navbar />
      <div>
        <Routes>
          <Route path="/Login" element={<Login />} />
          <Route path="/Register" element={<Register />} />

          {!localStorage.getItem('token') && (
            <Route path="*" element={<Navigate to="/Login" replace />} />
          )}

          {role === 'Student' && (
            <>
              <Route path="/" element={<ProtectedRoute><Home /></ProtectedRoute>} />
              <Route path="/Home" element={<ProtectedRoute><Home /></ProtectedRoute>} />
            </>
          )}

          {role === 'Docent' && (
            <>
              <Route path="/"             element={<ProtectedRoute><DocentHome /></ProtectedRoute>} />
              <Route path="/klas/:klasId" element={<ProtectedRoute><Klas /></ProtectedRoute>} />
              <Route path='/klas/:klasId/:studentId/:vakId' element={<ProtectedRoute><CijferWijzigen /></ProtectedRoute>}></Route>

              <Route path='/Overzicht' element={<ProtectedRoute><DocentenOverzicht /></ProtectedRoute>} />
              <Route path='/Klassen' element={<ProtectedRoute><DocentKlassen /></ProtectedRoute>} />
              <Route path='/KlassenWijzigen/:klasId' element={<ProtectedRoute><DocentKlassenWijzigen /></ProtectedRoute>} />
            </>
          )}
        </Routes>
      </div>
    </Router>
  )
}

export default App
