import React, { useState, useEffect } from "react";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";
import "../../css/DocentenOverzicht.css";

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

const DocentKoppelenEnOverzicht = () => {
    const [docenten, setDocenten] = useState([]);
    const [vakken, setVakken] = useState([]);
    const [klassen, setKlassen] = useState([]);
    const [selectedDocent, setSelectedDocent] = useState(null);
    const [selectedVak, setSelectedVak] = useState(null);
    const [selectedKlas, setSelectedKlas] = useState(null); 
    const [data, setData] = useState([]);

    const token = localStorage.getItem('token');
    const decoded = parseJwt(token);
    const role = decoded?.role;     

    const fetchOverzicht = () => {
        fetch("https://localhost:7083/api/DocentVakken/DocentenOverzicht")
            .then(res => res.json())
            .then(result => {
                console.log("DocentenOverzicht result:", result);
                if (Array.isArray(result)) {
                    setData(result);
                } else if (Array.isArray(result.data)) {
                    setData(result.data);
                } else {
                    console.error("Onverwacht formaat voor DocentenOverzicht:", result);
                    setData([]);
                }
            })
            .catch(err => console.error("Fout bij ophalen overzicht:", err));
    };

    useEffect(() => {
        fetch("https://localhost:7083/api/Docenten")
            .then(res => res.json())
            .then(setDocenten)
            .catch(err => console.error("Fout bij ophalen docenten:", err));

        fetch("https://localhost:7083/api/Vakken")
            .then(res => res.json())
            .then(setVakken)
            .catch(err => console.error("Fout bij ophalen vakken:", err));

        fetch("https://localhost:7083/api/Klassen")
            .then(res => res.json())
            .then(setKlassen)
            .catch(err => console.error("Fout bij ophalen klassen:", err));
        
        fetchOverzicht();
    }, []);

    const handleSubmit = (e) => {
        e.preventDefault();
        if (!selectedDocent || !selectedVak || !selectedKlas) {
            alert("Selecteer een docent, vak en klas");
            return;
        }

        fetch("https://localhost:7083/api/DocentVakken", {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                docentId: selectedDocent.id,
                vakId: selectedVak.id,
                klasId: selectedKlas.klasId
            })
        })
        .then(res => {
            if (res.ok) {
                alert("Succesvol gekoppeld!");
                fetchOverzicht();
            } else {
                alert("Fout bij koppelen");
            }
        })
        .catch(err => console.error("POST fout:", err));
    };

    const handleSaveAsPDF = () => {
        const doc = new jsPDF();
        autoTable(doc, {
            head: [["Vak", "Docent", "Klas"]],
            body: data.map(item => [item.vakNaam, item.docentNaam, item.klasNaam]),
        });
        doc.save("docenten_overzicht.pdf");
    };

    return (
        <div style={{ display: "flex", alignItems: "flex-start", width: "100%" }}>
            {/* Overzicht links */}
            <div style={{ marginRight: "20px" }}>
                <h2>Overzicht</h2>
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
                        {Array.isArray(data) && data.map((item, index) => (
                            <tr key={index}>
                                <td>{item.vakNaam}</td>
                                <td>{item.docentNaam}</td>
                                <td>{item.klasNaam}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            {/* Form rechts */}
            {role?.includes("Administrator") && (
                <div style={{ marginLeft: "20px" }}>
                    <h2>Docent koppelen</h2>
                    <form onSubmit={handleSubmit}>
                        <div style={{ display: "flex", flexDirection: "column", gap: "10px" }}>
                            <select
                                value={selectedDocent?.id || ""}
                                onChange={e => setSelectedDocent(docenten.find(d => d.id === parseInt(e.target.value)))}>
                                <option value="">Selecteer een docent</option>
                                {docenten.map(d => (
                                    <option key={d.id} value={d.id}>
                                        {d.voornaam} {d.achternaam}
                                    </option>
                                ))}
                            </select>

                            <select
                                value={selectedVak?.id || ""}
                                onChange={e => setSelectedVak(vakken.find(v => v.id === parseInt(e.target.value)))}>
                                <option value="">Selecteer een vak</option>
                                {vakken.map(v => (
                                    <option key={v.id} value={v.id}>{v.vakNaam}</option>
                                ))}
                            </select>

                            <select
                                value={selectedKlas?.klasId || ""}
                                onChange={e => setSelectedKlas(klassen.find(k => k.klasId === parseInt(e.target.value)))}>
                                <option value="">Selecteer een klas</option>
                                {klassen.map(k => (
                                    <option key={k.klasId} value={k.klasId}>{k.klasNaam}</option>
                                ))}
                            </select>
                        </div>
                        <div><button type="submit">Koppel</button></div>
                    </form>
                </div>
            )}
        </div>
    );
};

export default DocentKoppelenEnOverzicht;

