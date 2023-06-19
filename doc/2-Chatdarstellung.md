# Dokumentation von Chatdarstellung

Verbessern Sie die grundlegende Ausgabe der Chatnachrichten, so dass zusätzlich zum
Nutzernamen und dem Text auch die Uhrzeit des Sendens der Nachricht angezeigt wird.

Nutzen Sie zur Darstellung die den Nutzern zugewiesenen Farben.

## Zeitstempel

Jede Chatnachricht soll mit einem Zeitstempel versehen sein.

### Zuständig: Lucas Irchin

### Lösung:
- Bei jeder empfangen Nachricht gibt der MessageReceivedHandler zusätzlich die Zeit aus
- realisiert durch DateTime.Now.ToString("HH:mm");

### Anmerkung
- Die Zeitstempel haben bewusst keine Farbe, da es so übersichtlicher aussieht. Es wäre jedoch möglich gewesen.

## Farbige Nachrichten

Chatnachrichten werden in der Farbe des Senders abgebildet.

### Zuständig: Lucas Irchin

### Lösung:
- durch die übergebende Farbe in jeder ChatMessage kann jeder Nachricht mithilfe der Methode SetColor in ColorSettings gefärbt werden
- Hierbei wird jeder Name gefärbt, der ebenfalls in der ChatMessage enthalten ist
- damit belibt die Nachricht gut leserlich

## Nur benötigte Nachrichten

Alle für den Nutzer unötigen Nachrichten werden nicht mehr angezeigt.
### Zuständig: Lucas Irchin

### Lösung:
- verwirrende ausgaben wurden entfernt
- Nutzer sehen nur noch ihre und ander Nachrichten, sowie wichtige Meldungen (System: AFK Warnung)

## Hearthbeat gegen den Timeout

### Zuständig: Lucas Irchin

### Lösung:
- Im ChatServer wurde ein Timer eingbaut, damit alle 30 Sekunden ein Ping an alle wartenden Clients verschickt
- Somit wird verhindert, dass ein Client die Verbindung verliert und keine Nachrichten mehr empfangen kann