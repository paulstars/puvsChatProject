# Dokumentation von Benutzerverwaltung

Stellen Sie sicher, dass jeder Nutzername in einer Chatsession nur einmal verwendet werden
kann.

Ebenso soll jedem Teilnehmer eine individuelle und eindeutige Farbe zugeordnet
werden.

## Farbauswahl

Der Nutzer soll nach der Eingabe des Namens eine Farbe auswählen, dies ändert die Farbe des Nutzers im Chat.

Zuständig: Lucas Irchin
Lösung:

## Einmaliger Nutzername

Bei der Eingabe eines Namens soll überprüft werden, ob dieser bereits vergeben ist. Ist dies der Fall soll der Nutzer benachrichtigt werden, er solle sich einen anderen Namen aussuchen.

Zuständig: Paul Steinke
Lösung:
- im Server gibt es eine List "usedNames" in der alle aktiven Nutzernamen gespeichert werden
- neuer Task "ChooseName" im ChatClient fragt den user nach einem namen und schickt diesen in der url an MapGet"/names"
- neuer MapGet"/names" in Server, dem über die url der gewälte name übergeben wird
- MapGet überprüft ob gewählter name in usedNames(List) entahlten ist, wenn nicht wird er hinzugefügt und der response auf Status: 201  gesetzt
- wenn name nicht enthalten wird Status: 406 zurückgegeben
- Task "ChooseName" fragt nach einem neuen Namen vom user, so lange der response Status nicht zwischen 200 und 299 liegt
- beim Verlassen des Chats wird der Name aus der usedNames Liste gelöscht
- beim Senden einer Nachricht prüft der Server nun ob der Name registriert ist

Anmerkung:
- Ich habe mich für einen Get entschieden falls wir später noch die aktuelle Liste an Usern abfragen wollen.
