using Veenhoop.Models;
using Microsoft.EntityFrameworkCore;
using Bogus;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Veenhoop.Data
{
    public static class DataSeeder
    {
        public static void Seed(ApplicationDbContext context) // <-- HIER AANGEPAST: VeenhoopContext naar ApplicationDbContext
        {
            // Zorg ervoor dat de database is aangemaakt
            context.Database.EnsureCreated();

            // Voer alleen seeding uit als er nog geen data is
            // Controleer op bestaande data in belangrijke tabellen
            if (context.Vakken.Any() || context.Docenten.Any() || context.Gebruikers.Any() || context.Klassen.Any() || context.Toetsen.Any() || context.Cijfers.Any() || context.DocentVakken.Any())
            {
                Console.WriteLine("Database bevat al data. Seeding overgeslagen.");
                return; // Database bevat al data, geen seeding nodig.
            }

            // Start ID voor alle entiteiten
            int currentId = 20;

            // --- Vakken genereren (6-15) ---
            Console.WriteLine("Vakken genereren...");
            var vakkenFaker = new Faker<Vakken>() // Maak de Faker apart aan
                .CustomInstantiator(f => new Vakken
                {
                    Id = currentId++,
                    VakNaam = f.Hacker.Noun() // Gebruik een algemenere methode voor vaknaam
                });
            var vakken = vakkenFaker.Generate(new Faker().Random.Int(6, 15)); // <-- HIER AANGEPAST: f.Random.Int() naar new Faker().Random.Int()
            context.Vakken.AddRange(vakken);
            context.SaveChanges();
            Console.WriteLine($"Vakken gegenereerd: {vakken.Count}");

            // --- Docenten genereren (20) ---
            Console.WriteLine("Docenten genereren...");
            currentId = 20; // Reset currentId voor docenten
            var docenten = new Faker<Docenten>()
                .CustomInstantiator(f => new Docenten
                {
                    Id = currentId++,
                    Voornaam = f.Name.FirstName(),
                    Achternaam = f.Name.LastName(),
                    GeboorteDatum = DateOnly.FromDateTime(f.Date.Past(50, DateTime.Now.AddYears(-25))),
                    Stad = f.Address.City(),
                    Adres = f.Address.StreetAddress(),
                    Postcode = f.Address.ZipCode(),
                    Email = f.Internet.Email(), // Bogus kan zelf een email genereren
                    Wachtwoord = "asdfghjkl" // Vast wachtwoord
                })
                .RuleFor(d => d.Tussenvoegsel, f => f.Name.Suffix()) // Tussenvoegsel is nullable
                .Generate(20);
            context.Docenten.AddRange(docenten);
            context.SaveChanges();
            Console.WriteLine($"Docenten gegenereerd: {docenten.Count}");


            // --- Klassen genereren (10) ---
            Console.WriteLine("Klassen genereren...");
            currentId = 20; // Reset currentId voor klassen
            var klassen = new Faker<Klassen>()
                .CustomInstantiator(f => new Klassen
                {
                    Id = currentId++,
                    KlasNaam = $"{f.Random.Char('A', 'Z')}{f.Random.Number(1, 4)}{f.Random.Char('A', 'C')}",
                    DocentId = f.PickRandom(docenten).Id,
                    Docent = docenten.First(d => d.Id == f.PickRandom(docenten).Id) // Zorg voor de navigatie property
                })
                .Generate(10);
            context.Klassen.AddRange(klassen);
            context.SaveChanges();
            Console.WriteLine($"Klassen gegenereerd: {klassen.Count}");

            // --- Gebruikers genereren (30) en verdelen over klassen ---
            Console.WriteLine("Gebruikers genereren en toewijzen aan klassen...");
            currentId = 20; // Reset currentId voor gebruikers
            var gebruikers = new List<Gebruikers>();
            var studentenNummers = new HashSet<int>();

            for (int i = 0; i < 30; i++)
            {
                var gebruiker = new Faker<Gebruikers>()
                    .CustomInstantiator(f =>
                    {
                        int studentNum;
                        do
                        {
                            studentNum = f.Random.Number(10000, 99999);
                        } while (!studentenNummers.Add(studentNum)); // Zorg voor unieke studentnummers

                        var klasToAssign = f.PickRandom(klassen); // Kies een willekeurige klas

                        return new Gebruikers
                        {
                            Id = currentId++,
                            StudentenNummer = studentNum,
                            Voornaam = f.Name.FirstName(),
                            Achternaam = f.Name.LastName(),
                            GeboorteDatum = DateOnly.FromDateTime(f.Date.Past(20, DateTime.Now.AddYears(-16))),
                            Stad = f.Address.City(),
                            Adres = f.Address.StreetAddress(),
                            Postcode = f.Address.ZipCode(),
                            Email = f.Internet.Email(),
                            Wachtwoord = "asdfghjkl", // Vast wachtwoord
                            KlasId = klasToAssign.Id // Wijs student toe aan een willekeurige klas
                        };
                    })
                    .RuleFor(g => g.Tussenvoegsel, f => f.Name.Suffix()) // Tussenvoegsel is nullable
                    .Generate(1); // Genereer één gebruiker per keer om unieke studentnummers te garanderen
                gebruikers.Add(gebruiker.First());
            }
            context.Gebruikers.AddRange(gebruikers);
            context.SaveChanges();
            Console.WriteLine($"Gebruikers gegenereerd: {gebruikers.Count}");

            // Koppel de studenten aan de klassen (navigatie property Studenten)
            foreach (var klas in klassen)
            {
                klas.Studenten.AddRange(gebruikers.Where(g => g.KlasId == klas.Id));
            }
            context.SaveChanges();


            // --- DocentVakken koppelingen (elke docent aan ten minste één vak) ---
            Console.WriteLine("DocentVakken koppelingen genereren...");
            currentId = 20; // Reset currentId
            var docentVakken = new List<DocentVakken>();
            foreach (var docent in docenten)
            {
                var vak = vakken[new Random().Next(vakken.Count)]; // Koppel aan een willekeurig vak
                var klas = klassen[new Random().Next(klassen.Count)]; // Koppel aan een willekeurige klas
                docentVakken.Add(new DocentVakken
                {
                    Id = currentId++,
                    DocentId = docent.Id,
                    VakId = vak.Id,
                    KlasId = klas.Id
                });
            }
            context.DocentVakken.AddRange(docentVakken);
            context.SaveChanges();
            Console.WriteLine($"DocentVakken koppelingen gegenereerd: {docentVakken.Count}");

            // --- Toetsen genereren (10 per vak) ---
            Console.WriteLine("Toetsen genereren...");
            currentId = 20; // Reset currentId voor toetsen
            var toetsen = new List<Toetsen>();
            foreach (var vak in vakken)
            {
                var faker = new Faker<Toetsen>()
                    .CustomInstantiator(f => new Toetsen
                    {
                        Id = currentId++,
                        VakId = vak.Id,
                        Naam = $"{f.Hacker.Adjective()} {f.Hacker.Noun()} Toets {vak.VakNaam.Replace(" ", "")}",
                        Weging = f.Random.Int(1, 10)
                    });

                toetsen.AddRange(faker.Generate(10));
            }
            context.Toetsen.AddRange(toetsen);
            context.SaveChanges();
            Console.WriteLine($"Toetsen gegenereerd: {toetsen.Count}");

            // --- Cijfers genereren (voor alle studenten behalve één) ---
            Console.WriteLine("Cijfers genereren...");
            currentId = 20; // Reset currentId voor cijfers
            var cijfers = new List<Cijfers>();
            var studentenMetCijfers = gebruikers.Skip(1).ToList(); // Alle studenten behalve de eerste

            foreach (var student in studentenMetCijfers)
            {
                foreach (var toets in toetsen)
                {
                    var docentVoorCijfer = docenten.FirstOrDefault(); // Neem willekeurige docent voor cijfer
                    if (docentVoorCijfer != null)
                    {
                        cijfers.Add(new Cijfers
                        {
                            Id = currentId++,
                            DocentId = docentVoorCijfer.Id,
                            GebruikersId = student.Id,
                            ToetsId = toets.Id,
                            Cijfer = Math.Round(new Faker().Random.Decimal(1.0M, 10.0M), 1),
                            Datum = DateTimeOffset.Now.AddDays(new Faker().Random.Int(-365, 0)),
                            Leerjaar = new Faker().Random.Int(1, 4), // Voorbeeld leerjaar
                            Periode = new Faker().Random.Int(1, 4) // Voorbeeld periode
                        });
                    }
                }
            }
            context.Cijfers.AddRange(cijfers);
            context.SaveChanges();
            Console.WriteLine("Seeding voltooid!");
        }
    }
}