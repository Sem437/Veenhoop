import React, { useState, useEffect } from "react";

const Rol = () => {
    const [docenten, setDocenten] = useState([]);
    const [rollen, setRollen] = useState([]);
    const [selectedDocent, setSelectedDocent] = useState(null);
    const [selectedRol, setSelectedRol] = useState(null);

    useEffect(() => {
        fetch("https://localhost:7083/api/Docenten")
            .then(response => response.json())
            .then(data => setDocenten(data))
            .catch(error => console.error('Error fetching docenten:', error));

        fetch("https://localhost:7083/api/auth/Rol")
            .then(response => response.json())
            .then(data => setRollen(data))
            .catch(error => console.error('Error fetching rollen:', error));
    }, []);

    const handleSubmit = (e) => {
        e.preventDefault();
        if (!selectedDocent || !selectedRol) {
            alert("Selecteer een docent en een rol");
            return;
        }

        console.log(selectedDocent, selectedRol);
        fetch("https://localhost:7083/api/auth/rolGebruikers", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                userId: selectedDocent.id,
                rolId: selectedRol.id
            })
        })
        .then(response => {
            if (response.ok) {
                alert("Succesvol toegevoegd!");
            } else {
                return response.json().then(data => {
                    alert(data.error || 'Er is een fout opgetreden');
                });
            }
        })
        .catch(error => console.error('Error posting rolGebruikers:', error));
    };

    return (
        <form onSubmit={handleSubmit}>
            <select onChange={e => setSelectedDocent(docenten.find(docent => docent.id === parseInt(e.target.value)))}>
                <option value="">Selecteer een docent</option>
                {docenten.map(docent => (
                    <option key={docent.id} value={docent.id}>{docent.voornaam} {docent.achternaam}</option>
                ))}
            </select>
            <select onChange={e => setSelectedRol(rollen.find(rol => rol.id === parseInt(e.target.value)))}>
                <option value="">Selecteer een rol</option>
                {rollen.map(rol => (
                    <option key={rol.id} value={rol.id}>{rol.naam}</option>
                ))}
            </select>
            <button type="submit">Toevoegen</button>
        </form>
    );
}

export default Rol;

