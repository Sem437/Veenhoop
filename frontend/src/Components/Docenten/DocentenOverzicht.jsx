import React, { useState, useEffect } from "react";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";

const DocentenOverzicht = () => {
    const [data, setData] = useState([]);

    useEffect(() => {
        fetch("https://localhost:7083/api/DocentVakken/DocentenOverzicht")
            .then(res => res.json())
            .then(json => setData(json))
            .catch(err => console.error("Fout bij ophalen docenten:", err));
    }, []);

    const handleSaveAsPDF = () => {     
        
        const doc = new jsPDF();
        autoTable(doc, {
            head: [["Vak", "Docent", "Klas"]],
            body: data.map(item => [item.vakNaam, item.docentNaam, item.klasNaam]),
        });

        doc.save("docenten_overzicht.pdf");
    };

    return (
        <div>
            <button onClick={handleSaveAsPDF}>Opslaan als PDF</button>
            <table>
                <thead>
                    <tr>
                        <th>Vak</th>
                        <th>Docent</th>
                        <th>Klas</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) => (
                        <tr key={index}>
                            <td>{item.vakNaam}</td>
                            <td>{item.docentNaam}</td>
                            <td>{item.klasNaam}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default DocentenOverzicht;
