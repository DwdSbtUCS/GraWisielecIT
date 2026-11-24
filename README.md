# Gra Wisielec IT w oparciu o baze słów z IT — Python oraz C#

Konsolowa gra **Wisielec IT** napisana obiektowo w Pythonie oraz C#, wykorzystująca klasy `WordBank`, `Player` i `Game` oraz bazę 100 haseł związanych ze sprzętem komputerowym i IT w osobnym pliku `slowa.txt`.

## Funkcjonalności
- **`WordBank`** — wczytuje 100 haseł z pliku `slowa.txt` i losuje słowo do gry.
- **`Player`** — przechowuje imię gracza, podane litery oraz liczbę błędów.
- **`Game`** — zarządza rozgrywką: odsłanianiem liter, limitem błędów i stanem gry (`W trakcie` / `Wygrana` / `Przegrana`).

## Struktura repozytorium

```text
.
├── slowa.txt
├── python/
│   └── wisielec.py
├── csharp/
│   ├── wisielec.cs
│   └── Wisielec.csproj
└── README.md
```

## Uruchomienie

### Python
```bash
python python/wisielec.py
```

### C#
```bash
dotnet run --project csharp/Wisielec.csproj
```
(albo `dotnet run` wewnątrz folderu `csharp`)
