import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";

const parseJwt = (token) => {
  try {
    const base64Payload = token.split('.')[1];
    const payload = atob(base64Payload); // decode base64
    return JSON.parse(payload); // parse JSON
  } 
  catch (e) {
    console.error("Fout bij decoderen van token:", e);
    return null;
  }
};

const Klas = () => {
    const [data, setData] = useState(null); 
    const [toetsen, setToetsen] = useState([]);
    const { klasId } = useParams();

    const token = localStorage.getItem('token');
    const payload = parseJwt(token);
    const docentId = payload?.nameid;
    if (!docentId) {
        return;
    }

    const [formData, setFormData] = useState({
        leerjaar: "",
        periode: "",
        toetsId: "",
        cijfers: {} // { studentId: cijfer }
    });

    useEffect(() => {
        fetch(`https://localhost:7083/api/Docenten/klassen/${docentId}/${klasId}`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' },
        })
            .then(res => res.json())
            .then(json => {
                setData(json);
                if (json.vakId) {
                    fetch(`https://localhost:7083/api/Toetsen/Vakken/${json.vakId}`)
                        .then(res => res.json())
                        .then(setToetsen)
                        .catch(err => console.error("Fout bij toetsen ophalen:", err));
                }
            })
            .catch(err => console.error('Fout bij klas ophalen:', err));
    }, [klasId]);

    const handleCijferChange = (studentId, value) => {
        setFormData(prev => ({
            ...prev,
            cijfers: {
                ...prev.cijfers,
                [studentId]: value
            }
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();

        const payload = {
            leerjaar: formData.leerjaar,
            periode: formData.periode,
            toetsId: formData.toetsId,
            klasId: data.klasId,
            cijfers: Object.entries(formData.cijfers).map(([studentId, cijfer]) => ({
                studentId: parseInt(studentId),
                cijfer: parseFloat(cijfer)
            }))
        };

        console.log("Versturen:", payload);

        fetch("https://localhost:7083/api/Docenten/klas", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        })
            .then(res => {
                if (res.ok) {
                    alert("Cijfers succesvol opgeslagen!");
                } else {
                    alert("Fout bij opslaan.");
                }
            })
            .catch(err => {
                console.error("Fout bij POST:", err);
            });
    };

    return (
        <div>
            {data && (
                <div>
                    <h1>Klas: {data.klasNaam}</h1>
                    <h3>Vak: {data.vakNaam}</h3>

                    <form onSubmit={handleSubmit}>
                        <div>
                            <input
                                type="number"
                                placeholder="Leerjaar"
                                min="1"
                                max="6"
                                value={formData.leerjaar}
                                onChange={(e) => setFormData({ ...formData, leerjaar: e.target.value })}
                            />

                            <select
                                value={formData.periode}
                                onChange={(e) => setFormData({ ...formData, periode: e.target.value })}
                            >
                                <option value="1">Periode 1</option>
                                <option value="2">Periode 2</option>
                                <option value="3">Periode 3</option>
                                <option value="4">Periode 4</option>
                            </select>

                            <select
                                value={formData.toetsId}
                                onChange={(e) => setFormData({ ...formData, toetsId: e.target.value })}
                            >
                                <option value="">Kies toets</option>
                                {toetsen.map((toets) => (
                                    <option key={toets.id} value={toets.id}>
                                        {toets.naam}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <table>
                            <thead>
                                <tr>
                                    <th>Leerling</th>
                                    <th>Cijfer</th>
                                </tr>
                            </thead>
                            <tbody>
                                {data.studenten.map((student) => (
                                    <tr key={student.id}>
                                        <td>{student.voornaam} {student.tussenvoegsel} {student.achternaam}</td>
                                        <td>
                                            <input
                                                type="number"
                                                min="1"
                                                max="10"
                                                step="0.1"
                                                value={formData.cijfers[student.id] || ""}
                                                onChange={(e) => handleCijferChange(student.id, e.target.value)}
                                            />
                                        </td>
                                        <td>
                                            <a href={`/klas/${klasId}/${student.id}/${data.vakId}`}>Cijfer wijzigen</a>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>

                        <button type="submit">Cijfers opslaan</button>
                    </form>
                </div>
            )}
        </div>
    );
};

export default Klas;
