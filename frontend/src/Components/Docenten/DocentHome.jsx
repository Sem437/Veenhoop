import React, { use, useEffect, useState} from "react";

const DocentHome = () => {
    const[data, setData] = useState([]);

    useEffect(() => {
        fetch('https://localhost:7083/api/Docenten/klassen/1',{
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',                
            },
        }).then(response => response.json())
        .then(json => { setData(json); }) 
        .catch(error => console.error('Error:', error));
    }, []);
    
    return (
        <div>
            <table>
                <thead>
                    <tr>
                        <th>Klas</th>
                        <th>Vak</th>
                        <th>Studenten</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) => (
                        <tr key={index}>
                            <td>{item.klasNaam}</td>
                            <td>{item.vakNaam}</td>
                            <td>
                                {item.studenten.map((student, index) => (
                                    <div key={index}>
                                        {student.voornaam} {student.tussenvoegsel} {student.achternaam}
                                    </div>
                                ))}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
    
        </div>
    
    );
}



export default DocentHome
