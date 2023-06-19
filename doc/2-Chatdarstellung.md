# Dokumentation von Chatdarstellung

Verbessern Sie die grundlegende Ausgabe der Chatnachrichten, so dass zusätzlich zum
Nutzernamen und dem Text auch die Uhrzeit des Sendens der Nachricht angezeigt wird.

Nutzen Sie zur Darstellung die den Nutzern zugewiesenen Farben.

## Zeitstempel

Jede Chatnachricht soll mit einem Zeitstempel versehen sein.

### Zuständig: Lucas Irchin

### Lösung:
- Datei: Program.cs
- Stelle(n): MessageReceivedHandler()
- Änderung: Methode **DateTime.Now** gibt eine *DateTime* Variable zurück (15.06.2023 14:00:00). Dieser Wert wird in ein String konvertiert. Das Datum und die Sekunden werden noch mithilfe der **string.Remove()** Methode entfernt. Die Ausgabe erfolgt vor dem Namen im **MessageReceivedHandler()**.

## Farbige Nachrichten

Chatnachrichten werden in der Farbe des Senders abgebildet.

### Zuständig: Lucas Irchin

### Lösung:

## Nur benötigte Nachrichten

Alle für den Nutzer unötigen Nachrichten werden nicht mehr angezeigt.

### Lösung:
- Datei: Program.cs
- Stelle(n): Bereich MArkkiert mit "// query the user for messages to send or the exit command"
- Änderung: Folgende Nachrichten wurden auskommentiert
  - //Console.WriteLine($"Sending message: {content}");
  - //Console.WriteLine("Message sent successfully.");

## Keine doppelte Nachrichten

## Hearthbeat gegen den Timeout
