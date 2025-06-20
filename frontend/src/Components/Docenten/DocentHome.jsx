import React, { use, useEffect, useState} from "react";

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

const DocentHome = () => {
    const[data, setData] = useState([]);

    useEffect(() => {
        const token = localStorage.getItem('token');

        const payload = parseJwt(token);
        const gebruikersId = payload?.nameid;

        if (!gebruikersId)
        {
            return;
        }

        fetch(`https://localhost:7083/api/Docenten/klassen/${gebruikersId}`,{
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
                        <th>Aantal studenten</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) => (
                        <tr key={index}>
                            <td>{item.klasNaam}</td>
                            <td>{item.vakNaam}</td>
                            <td>
                                {item.studenten.length}
                            </td>
                             <td><a href={`/klas/${item.id}`}>Cijfers invoeren</a></td>
                        </tr>
                    ))}
                </tbody>
            </table>
    
        </div>
    
    );
}



export default DocentHome
