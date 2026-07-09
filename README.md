# 🍴 Rezept Hopper

Ein digitales Kochbuch mit KI-gestützter Rezept-Extraktion. Füge einfach eine URL ein und die App extrahiert automatisch alle Rezeptinformationen.

## ✨ Features

- **KI-Rezept-Import** – URL eingeben, Gemini AI extrahiert das Rezept automatisch
- **Rezeptverwaltung** – Erstellen, bearbeiten, löschen
- **Ordner** – Rezepte in Kategorien organisieren
- **Teilen** – Ordner mit anderen Nutzern teilen (Lesen oder Bearbeiten)
- **Benutzerkonten** – Sichere Authentifizierung mit Passwort-Hashing

## 🛠️ Tech Stack

- **Blazor** (.NET 10) – Interactive Server-Side Rendering
- **Entity Framework Core** – SQLite Datenbank
- **Google Gemini 2.5 Flash** – KI für Rezept-Extraktion
- **Bootstrap 5** – UI Framework
- **Docker** – Container-Deployment

## 🚀 Schnellstart

### Voraussetzungen

- [Docker](https://docs.docker.com/get-docker/) installiert
- [Gemini API Key](https://aistudio.google.com/) (kostenlos)

### Installation

1. **Repository klonen:**
   ```sh
   git clone https://github.com/NonsinDev/Rezept-Hopper.git
   cd Rezept-Hopper
   ```

2. **API-Key einrichten:**
   ```sh
   cp .env.example .env
   nano .env  # Deinen Gemini API-Key eintragen
   ```

3. **Container starten:**
   ```sh
   docker compose up -d --build
   ```

4. **Öffnen:** [http://localhost:8080](http://localhost:8080)

## 📁 Projektstruktur

```
Rezept Hopper/
├── Components/
│   ├── Layout/          # NavMenu, MainLayout
│   └── Pages/           # Alle Seiten (Rezepte, Ordner, Login, etc.)
├── Data/
│   ├── Models/          # User, Recipe, Folder, etc.
│   └── AppDbContext.cs  # EF Core Context
├── Services/            # AuthService, GeminiService, etc.
├── Prompts/             # KI-Prompt für Rezept-Extraktion
└── Program.cs           # App-Konfiguration
```

## ⚙️ Konfiguration

### Umgebungsvariablen

| Variable | Beschreibung |
|----------|-------------|
| `Gemini__ApiKey` | Google Gemini API Key |
| `ConnectionStrings__DefaultConnection` | SQLite Pfad (Standard: `/app/data/rezepthopper.db`) |

### Lokale Entwicklung

1. **.NET 10 SDK** installieren
2. API-Key in `appsettings.json` eintragen (nur lokal!)
3. `dotnet run` im `Rezept Hopper` Ordner

## 🐳 Docker Befehle

```sh
# Starten
docker compose up -d --build

# Logs ansehen
docker compose logs -f

# Stoppen
docker compose down

# DB-Backup
docker cp rezepthopper:/app/data/rezepthopper.db ./backup.db
```

## 🔒 Sicherheit

- Passwörter werden mit **PBKDF2** (200.000 Iterationen, SHA256) gehasht
- API-Keys **niemals** im Code committen – immer `.env` verwenden
- SQLite-Datenbank wird in einem Docker Volume persistiert

## 📝 KI-Prompt anpassen

Der Prompt für die Rezept-Extraktion liegt in `Prompts/recipe-extraction.md`. Du kannst ihn anpassen um:
- Andere Sprachen zu unterstützen
- Zusätzliche Felder zu extrahieren
- Das Ausgabeformat zu ändern

## 📄 Lizenz

MIT License

---

Made with ❤️ and Blazor
