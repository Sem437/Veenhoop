import React, { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'

  const parseJwt = (token) => {
    try {
        const base64Payload = token.split('.')[1];
        const payload = atob(base64Payload);
        return JSON.parse(payload);
    } catch (e) {
        console.error("Token decoding fout:", e);
        return null;
    }
    };

const CijferWijzigen = () => {
    const [data, setData] = useState([])
    const { studentId, vakId } = useParams()
    const token = localStorage.getItem('token')
    const decoded = parseJwt(token);
    const docentId = decoded?.nameid;  

    useEffect(() => {
        fetch(`https://localhost:7083/api/Docenten/Studentcijfers/${studentId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'docentId': docentId,
                'vakId': vakId,
                'Authorization': `Bearer ${token}`,          
            },
        })
        .then(res => res.json())
        .then(json => {
            setData(json);
        })
        .catch(err => console.error('Fout bij cijfers ophalen:', err));
    }, [studentId, docentId, vakId, token])

    return (
        <div>
            <h1>Cijfers van {studentId}</h1>
            <table>
                <thead>
                    <tr>
                        <th>Vak</th>
                        <th>Toets</th>
                        <th>Cijfer</th>
                        <th>Leerjaar</th>
                        <th>Periode</th>
                        <th>Voornaam</th>
                        <th>Achternaam</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map(item => (
                        <tr key={item.cijferId}>
                            <td>{item.vakNaam}</td>
                            <td>{item.toetsNaam}</td>
                            <td>{item.cijfer}</td>
                            <td>{item.leerjaar}</td>
                            <td>{item.periode}</td>
                            <td>{item.voornaam}</td>
                            <td>{item.achternaam}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}

export default CijferWijzigen
