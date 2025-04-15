import React, { useState } from 'react';

const Register = () => {
  const [formData, setFormData] = useState({
    voornaam: '',
    tussenvoegsel: '',
    achternaam: '',
    geboortedatum: '',
    email: '',
    wachtwoord: ''
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [e.targetname]: value
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log('Form verstuurd:', formData);
    // hier kun je straks data opsturen naar backend of validatie doen
  };

  return (
    <div>
      <h2>Registreer</h2>
      <form onSubmit={handleSubmit}>
        <input type="text" name="voornaam" placeholder="Voornaam" required/>
        <input type="text" name="tussenvoegsel" placeholder="Tussenvoegsel" />
        <input type="text" name="achternaam" placeholder="Achternaam" required/>
        <input type="date" name="geboortedatum" placeholder="Geboortedatum" required/>
        <input type="text" name='stad' placeholder="Stad" required />
        <input type="text" name='adres' placeholder="Adres" required />
        <input type="text" name='postcode' placeholder="Postcode" required />
        <input type="password" name="wachtwoord" placeholder="Wachtwoord" required/>
        <input type="email" name="email" placeholder="Email" required/>
        <input type='submit' value='Registreer' />  
      </form>
    </div>
    );

};

export default Register;
