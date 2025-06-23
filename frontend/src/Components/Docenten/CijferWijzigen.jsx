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
    const [student, setStudent] = useState(null)
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

        fetch(`https://localhost:7083/api/Gebruikers/${studentId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
        })
        .then(res => res.json())
        .then(json => setStudent(json))
        .catch(err => console.error('Fout bij student ophalen:', err));
    }, [studentId, docentId, vakId, token])

    const handleSafe = async (item) => {
        try{
            const response = await fetch(`https://localhost:7083/api/Cijfers/${item.cijferId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                },
                body: JSON.stringify({  
                    Id: item.cijferId,              
                    docentId: docentId,
                    gebruikersId: studentId,
                    toetsId: item.toetsId,
                    cijfer: item.cijfer,
                    datum: item.datum,
                    leerjaar: item.leerjaar,
                    periode: item.periode,
                }),
            });

            if (!response.ok) {
                throw new Error('Netwerk fout bij opslaan cijfer');
            }

            const result = await response.json();
            alert('Cijfer succesvol opgeslagen:', result);            
        }
        catch (error) {
            console.error('Fout bij opslaan cijfer:', error);
        }
    }

    return (
        <div>
            <h1>Cijfers van {student ? `${student.voornaam} ${student.achternaam}` : 'Laden...'}</h1>
            <table>
                <thead>
                    <tr>
                        <th>Vak</th>
                        <th>Toets</th>
                        <th>Cijfer</th>
                        <th>Leerjaar</th>
                        <th>Periode</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map(item => (
                        <tr key={item.cijferId}>
                            <td>{item.vakNaam}</td>
                            <td>{item.toetsNaam}</td>
                            <td>
                                <input value={item.cijfer} step={"0.1"} type='number' max={"10"} min={"1"} 
                                onChange={e => setData(data.map(item2 => item2.cijferId === item.cijferId ? {...item2, cijfer: parseFloat(e.target.value)} : item2))} ></input>
                            </td>
                            <td>{item.leerjaar}</td>
                            <td>{item.periode}</td>
                            <td><button onClick={() => handleSafe(item)}>Opslaan</button></td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}

export default CijferWijzigen

