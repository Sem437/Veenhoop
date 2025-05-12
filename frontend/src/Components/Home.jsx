import React, { useEffect, useState } from 'react';

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
      {data.length > 0 ? (
        <div>
          {data.map((item, index) => (
            <div key={index} >
              <p><strong>Vak:</strong> {item.vakkenNaam}</p>
              <p><strong>Toets:</strong> {item.toetsNaam}</p>
              <p><strong>Cijfer:</strong> {item.cijfer}</p>
            </div>
          ))}
        </div>
      ) : (
        <p>Data wordt geladen...</p>
      )}
    </div>
  );
};

export default Home;
