import React, { useState } from 'react';
import '../css/Register.css';

const Register = () => {
  const [formData, setFormData] = useState({
    voornaam: '',
    tussenvoegsel: '',
    achternaam: '',
    geboortedatum: '',
    email: '',
    wachtwoord: '',
    stad: '',
    adres: '',
    postcode: ''
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch('https://localhost:7083/api/Auth/Register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      const data = await response.json();

      if (response.ok) {
        console.log('Registratie succesvol:', data);
        alert('Registratie gelukt!');
      } else {
        console.error('Registratie mislukt:', data);
        alert('Er ging iets mis bij registratie.');
      }
    } catch (error) {
      console.error('Fout bij registratie:', error);
      alert('Netwerkfout of server down');
    }
  };

  return (
    <div className="register-container">
      <h2>Registreer</h2>
      <form onSubmit={handleSubmit}>
        <input type="text" name="voornaam" placeholder="Voornaam" value={formData.voornaam} onChange={handleChange} required />
        <input type="text" name="tussenvoegsel" placeholder="Tussenvoegsel" value={formData.tussenvoegsel} onChange={handleChange} />
        <input type="text" name="achternaam" placeholder="Achternaam" value={formData.achternaam} onChange={handleChange} required />
        <input type="date" name="geboortedatum" placeholder="Geboortedatum" value={formData.geboortedatum} onChange={handleChange} required />
        <input type="text" name="stad" placeholder="Stad" value={formData.stad} onChange={handleChange} required />
        <input type="text" name="adres" placeholder="Adres" value={formData.adres} onChange={handleChange} required />
        <input type="text" name="postcode" placeholder="Postcode" value={formData.postcode} onChange={handleChange} required />
        <input type="email" name="email" placeholder="Email" value={formData.email} onChange={handleChange} required />
        <input type="password" name="wachtwoord" placeholder="Wachtwoord" value={formData.wachtwoord} onChange={handleChange} required />
        <input type="submit" value="Registreer" />
      </form>
    </div>
  );
};



export default Register;
