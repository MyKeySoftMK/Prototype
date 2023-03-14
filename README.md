# Prototype
WPF-Blazor-Hybrid Prototyp

Hier entsteht der Prototyp einer WPF-Blazor Hybrid App mit dynamischer Nachladung von Modulen und nach MVVM Prinzipien

Aktueller Stand: unvollständig 

01_WPF

Stand: Ein "leeres" WPF-C#-Projekt - dort soll der Code rein, der die Blazor-App startet

10_Anwendung-Blazor

Stand: Das ist das aktuelle Blazor-Programm. Erstellt nach MVVM-Prinzipien. Enthält eine "Settings"-Seite, womit man dann Einstellungen je nach Art (Benutzer, Client und Anwendungsweit) einstellen kann

Zusätzlich ist noch implementiert, dass weitere Module dynamisch nachgeladen werden und dann im Menübaum auch angezeigt werden

20_Modul - Cars

Stand: Hier handelt es sich um ein simples Beispiel für ein Modul. Es hat keine weiteren Funktionen, sondern dient nur der Anzeige einer weiteren "Funktion" (hier z.B. das Anlegen von neuen Fahrzeugen)

Anmerkung: Aktuell kommt es da zu einem Fehler, weil ja keine DB-Anbindung ist

90_Framework

Hier sind die von mir entwickelten Projekte die mein Framework darstellen. Um das Haupt-Projekt nicht zu unübersichtlich zu machen, wollte ich die aktuellen DLLs angebinden - zu finden in dem Verzeichnis DLL. Aber anscheinend klappt da was mit dem "Auflösen" der Referenzen nicht, darum muss ich einige Projekte doch hier ablegen
