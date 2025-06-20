import React, { useState, useEffect } from 'react';

const KlassenTable = () => {
    const [klassen, setKlassen] = useState([]);

    useEffect(() => {
        fetch('https://localhost:7083/api/Klassen', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'                
            }
        })
        .then(response => response.json())
        .then(data => setKlassen(data))
        .catch(error => console.error('Error fetching klassen:', error));
    }, []);

    return (
        <table style={{ marginBlock: "4rem", width: "100%" }}>
            <thead>
                <tr>                 
                    <th>Klas Naam</th>
                    <th>Aantal Studenten</th>
                </tr>
            </thead>
            <tbody>
                {klassen.map(klas => (
                    <tr key={klas.klasId}>
                        <td>{klas.klasNaam}</td>
                        <td>{klas.studenten.length}</td>
                        <td><a href={`/KlassenWijzigen/${klas.klasId}`}>Studenten toevegen</a></td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
};

export default KlassenTable;

