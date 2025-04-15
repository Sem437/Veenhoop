import React, { useEffect, useState } from "react";
import axios from "axios";

const Cijfers = () => {
  const [cijfers, setCijfers] = useState([]);

  useEffect(() => {
    axios.get("https://localhost:7083/api/cijfers")
      .then((response) => {
        setCijfers(response.data);
      })
      .catch((error) => {
        console.error("Error fetching cijfers:", error);
      });
  }, []);

  return (
    <div>
      <h2>Cijfers</h2>
      <ul>
        {cijfers.map((cijfer) => (
          <li key={cijfer.id}>
            Gebruiker ID: {cijfer.gebruikersId} - Cijfer: {cijfer.cijfer}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default Cijfers;
