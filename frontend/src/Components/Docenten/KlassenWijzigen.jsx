import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';

const KlassenWijzigen = () => {
    const [klas, setKlas] = useState(null);
    const [studenten, setStudenten] = useState([]);
    const { klasId } = useParams();

    useEffect(() => {
        fetch(`https://localhost:7083/api/Klassen/${klasId}`)
            .then(res => res.json())
            .then(json => setKlas(json))
            .catch(err => console.error('Fout bij klas ophalen:', err));

        fetch("https://localhost:7083/api/Gebruikers")
            .then(res => res.json())
            .then(json => setStudenten(json))
            .catch(err => console.error('Fout bij studenten ophalen:', err));
    }, [klasId]);

    return (
        <div>
            <h1>{klas ? klas.klasNaam : 'Laden...'}</h1>
            <h2>Studenten in deze klas:</h2>
            <ul>
                {klas && klas.studenten.map(student => (
                    <p>{student.voornaam} {student.achternaam}</p>
                ))}
            </ul>

            <h2>Voeg een student toe aan deze klas</h2>
            <select id="studentSelect">
                {studenten.map(student => (
                    <option key={student.id} value={student.id}>
                        {student.voornaam} {student.achternaam} 
                    </option>
                ))}
            </select>
            <button onClick={() => {
                const select = document.getElementById('studentSelect');
                const studentId = select.value;
                fetch(`https://localhost:7083/api/Klassen/${klasId}/Studenten`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'studentId': studentId,

                    },
                })
                .then(res => res.json())
                .then(() => {
                    alert('Student toegevoegd!');
                    window.location.reload();
                })
                .catch(err => console.error('Fout bij toevoegen student:', err));                
            }}>Voeg toe</button>
        </div>
    );
};

export default KlassenWijzigen;
