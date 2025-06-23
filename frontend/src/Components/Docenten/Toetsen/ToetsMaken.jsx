import React, { useState, useEffect } from 'react';
import "../../../css/ToetsMaken.css";

const ToetsMaken = () => {
    const [vakken, setVakken] = useState([]);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetch('https://localhost:7083/api/Vakken', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'                
            }
        })
        .then(response => response.json())
        .then(data => setVakken(data))
        .catch(error => console.error('Error fetching vakken:', error));
    }, []);

    const handleSubmit = (e) => {
        e.preventDefault();
        const { naam, wegin, vakId } = e.target.elements;
        fetch('https://localhost:7083/api/Toetsen', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                naam: naam.value,
                wegin: wegin.value,
                vakId: vakId.value
            }),
        })
        .then(response => {
            if (response.ok) {
                alert('Toets succesvol aangemaakt!');
                window.location.reload();
            } else {
                return response.json().then(data => {
                    setError(data.message || 'Er is een fout opgetreden');
                });
            }
        })
        .catch(error => {
            setError('Er is een netwerkfout opgetreden');
            console.error('Error adding toets:', error);
        });
    };

    return (
        <div>
            {error && <p style={{ color: 'red' }}>{error}</p>}
            <form onSubmit={handleSubmit}>
                <label>
                    Naam:
                    <input type="text" name="naam" required/>
                </label>
                <br />
                <label>
                    Weging:
                    <input type="number" name="wegin" min="1" max="5" required/>
                </label>
                <br />
                <label>
                    Vak:
                    <select name="vakId">
                        {vakken.map(vak => (
                            <option key={vak.id} value={vak.id}>{vak.vakNaam}</option>
                        ))}
                    </select>
                </label>
                <br />
                <button type="submit">Maak toets</button>
            </form>
        </div>
    );
};

export default ToetsMaken;

