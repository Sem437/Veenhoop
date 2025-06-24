import React, { useState } from 'react';

const DocentMaken = () => {
    const [voornaam, setVoornaam] = useState('');
    const [tussenvoegsel, setTv] = useState('');
    const [achternaam, setAchternaam] = useState('');
    const [email, setEmail] = useState('');
    const [wachtwoord, setWachtwoord] = useState('');
    const [geboortedatum, setGeboortedatum] = useState('');
    const [stad, setStad] = useState('');
    const [adres, setAdres] = useState('');
    const [postcode, setPostcode] = useState('');

    const handleSubmit = (e) => {
        e.preventDefault();
        const payload = {
            voornaam,
            tussenvoegsel,
            achternaam,
            email,
            wachtwoord,
            geboortedatum,
            stad,
            adres,
            postcode
        };

        fetch("https://localhost:7083/api/Docenten", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        })
            .then(res => {
                if (res.ok) {
                    alert("Docent succesvol toegevoegd!");
                    window.location.reload();
                } else {
                    alert("Fout bij toevoegen.");
                    console.error("Fout bij POST:", res.statusText);
                }
            })
            .catch(err => {
                console.error("Fout bij POST:", err);
            });
    };

    return (
        <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', width: '300px',  }}>
            <label>
                Voornaam:
                <input type="text" value={voornaam} onChange={e => setVoornaam(e.target.value)} required/>
            </label>
            <label >
                Tussenvoegsel:
                <input type="text" value={tussenvoegsel} onChange={e => setTv(e.target.value)}/>
            </label>
            <label>
                Achternaam:
                <input type="text" value={achternaam} onChange={e => setAchternaam(e.target.value)} required/>
            </label>
            <label>
                E-mail:
                <input type="text" value={email} onChange={e => setEmail(e.target.value)} required/>
            </label>
            <label>
                Wachtwoord:
                <input type="password" value={wachtwoord} onChange={e => setWachtwoord(e.target.value)} required/>
            </label>
            <label>
                Geboortedatum:
                <input type="date" value={geboortedatum} onChange={e => setGeboortedatum(e.target.value)} required/>
            </label>
            <label >
                Stad:
                <input type="text" value={stad} onChange={e => setStad(e.target.value)} required/>
            </label>
            <label>
                Adres:
                <input type="text" value={adres} onChange={e => setAdres(e.target.value)} required/>
            </label>
            <label>
                Postcode:
                <input type="text" value={postcode} onChange={e => setPostcode(e.target.value)} required/>
            </label>

            <button type="submit">Maak Docent</button>
        </form>
    );
};

export default DocentMaken;
