import React, { useEffect, useState } from 'react';
import "../css/Home.css";

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

const Home = () => {
  const [data, setData] = useState([]);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) 
    {
      window.location.href = '/login';
      return;
    }

    const payload = parseJwt(token);
    const gebruikersId = payload?.nameid;

    if (!gebruikersId) 
    {
      console.error("Geen gebruikersId gevonden in token");
      return;
    }

    fetch(`https://localhost:7083/api/Cijfers/user/${gebruikersId}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    }).then(response => response.json())
      .then(json => { setData(json); }) //
      .catch(error => console.error('Error:', error));
  }, []);

  return (
    <div>
      <table>
        <thead>
          <tr>
            <th>Vak</th>
            <th>Periode 1</th>
            <th>Gemiddelde periode 1</th>
            <th>Periode 2</th>
            <th>Gemiddelde periode 2</th>
            <th>Periode 3</th>
            <th>Gemiddelde periode 3</th>
            <th>Periode 4</th>
            <th>Gemiddelde periode 4</th>
            <th>Gemiddelde</th>
          </tr>      
        </thead>
        <tbody>
          {data.map((item, index) => (
            <tr key={index}>
              <td>{item.vaknaam}</td>
              <td>{item.periode1}</td>
              <td>{item.gemPeriode1}</td>
              <td>{item.periode2}</td>
              <td>{item.gemPeriode2}</td>
              <td>{item.periode3}</td>
              <td>{item.gemPeriode3}</td>
              <td>{item.periode4}</td>
              <td>{item.gemPeriode4}</td>
              <td>{item.gem}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default Home;
