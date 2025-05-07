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
    postcode: '',
    klasId: 1,
    studentenNummer: 0
  });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => 
  {
    e.preventDefault();

    try {
      const response = await fetch('https://localhost:7083/api/Gebruikers', {
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

      if (response.ok) 
      {
        window.location.reload();
        alert('Registratie gelukt!');
      } 
      else 
      {
        console.error('Registratie mislukt:', data);
        alert('Er ging iets mis bij registratie.');
      }
    }
     
    catch (error) 
    {
      console.error('Fout bij registratie:', error);
      alert('Fout bij registratie:', error);
    }
  };

  return (
    <div className="register-container">
      <h2>Registreer</h2>
      <div>
        <form onSubmit={handleSubmit} className="register-form">
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
    </div>
  );
};



export default Register;
