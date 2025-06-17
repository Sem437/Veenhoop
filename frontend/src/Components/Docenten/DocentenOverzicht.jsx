import React, { useState, useEffect } from "react";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";

const DocentKoppelenEnOverzicht = () => {
    const [docenten, setDocenten] = useState([]);
    const [vakken, setVakken] = useState([]);
    const [klassen, setKlassen] = useState([]);
    const [selectedDocent, setSelectedDocent] = useState(null);
    const [selectedVak, setSelectedVak] = useState(null);
    const [selectedKlas, setSelectedKlas] = useState(null);
    const [data, setData] = useState([]);

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
        fetch("https://localhost:7083/api/DocentVakken/DocentenOverzicht")
            .then(res => res.json())
            .then(setData)
            .catch(err => console.error("Fout bij ophalen overzicht:", err));
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
                klasId: selectedKlas.id
            })
        })
        .then(res => {
            if (res.ok) {
                alert("Succesvol gekoppeld!");
                // Refresh data
                fetch("https://localhost:7083/api/DocentVakken/DocentenOverzicht")
                    .then(res => res.json())
                    .then(setData);
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
            <div style={{ marginRight: "20px", }}>
                <h2>Overzicht</h2>
                <button onClick={handleSaveAsPDF}>Opslaan als PDF</button>
                <table border="1" cellPadding="5" style={{ marginTop: "10px", width: "100%", borderCollapse: "collapse" }}>
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

            {/* Form rechts */}
            <div style={{ marginLeft: "20px"}}>
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
                            value={selectedKlas?.id || ""}
                            onChange={e => setSelectedKlas(klassen.find(k => k.id === parseInt(e.target.value)))}>
                        
                            <option value="">Selecteer een klas</option>
                            {klassen.map(k => (
                                <option key={k.id} value={k.id}>{k.klasNaam}</option>
                            ))}
                        </select>
                    </div>
                    <div><button type="submit">Koppel</button></div>                    
                </form>
            </div>
        </div>
    );
};

export default DocentKoppelenEnOverzicht;
