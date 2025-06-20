import React, { useEffect, useState } from 'react';
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";
import "../css/Home.css";

const parseJwt = (token) => {
  try {
    const base64Payload = token.split('.')[1];
    const payload = atob(base64Payload);
    return JSON.parse(payload);
  } catch (e) {
    console.error("Fout bij decoderen van token:", e);
    return null;
  }
};

const Home = () => {
  const [data, setData] = useState([]);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) {
      window.location.href = '/login';
      return;
    }

    const payload = parseJwt(token);
    const gebruikersId = payload?.nameid;

    if (!gebruikersId) {
      console.error("Geen gebruikersId gevonden in token");
      return;
    }

    fetch(`https://localhost:7083/api/Cijfers/user/${gebruikersId}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    })
      .then(response => response.json())
      .then(json => setData(json))
      .catch(error => console.error('Error:', error));
  }, []);

  const handleSaveAsPDF = () => {
    const doc = new jsPDF();

    autoTable(doc, {
      head: [[
        "Vak", "P1", "Gem. P1", 
        "P2", "Gem. P2", 
        "P3", "Gem. P3", 
        "P4", "Gem. P4", 
        "Gemiddelde"
      ]],
      body: data.map(item => [
        item.vaknaam,
        item.periode1,
        item.gemPeriode1,
        item.periode2,
        item.gemPeriode2,
        item.periode3,
        item.gemPeriode3,
        item.periode4,
        item.gemPeriode4,
        item.gem
      ]),
    });

    doc.save("cijfers_overzicht.pdf");
  };

  return (
    <div>
      <h2>Cijferoverzicht</h2>
      <button onClick={handleSaveAsPDF}>Opslaan als PDF</button>
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