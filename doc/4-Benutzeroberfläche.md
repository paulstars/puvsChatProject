# Dokumentation der Benutzeroberfläche

Verbessern Sie die Benutzeroberfläche der Chat-Anwendung, um eine benutzerfreundlichere
Erfahrung zu bieten.

Fügen Sie Funktionen wie automatisches Scrollen und
Echtzeitaktualisierungen hinzu, um neue Nachrichten ohne manuelles Aktualisieren
anzuzeigen.

## Allgemeine Anmerkung
Wir haben uns dazu entschieden die allgemeine Oberfläche in diesem Issue anzupassen

## Login Prozess

### Zuständig: Lucas Irchin

### Lösung:
- Mithife der TextSnippets Klasse ist es möglich alle Text in ihr dynamisch anzupasssen.
- dies geschieht mit den Methoden: WriteText und DeleteText
- Dafür wurd alle Texte in Arrays geschrieben, um sie zeilenweise ausgeben zu lassen

- In den Prozessen der Namensauswahl und Farbauswahl wurden Falsche eingaben abgefangen und durch eine änderung der Farbe der Elemente hervorgehoben
- Neue Eingaben werden daraufhin verlangt

- Es können keine leeren NAchrichten mehr versendet werden
- Bei leerer Eingabe springt der Curser wieder nach oben und es wird nichts versendet
