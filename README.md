# Prototype
WPF-Blazor-Hybrid Prototyp

Hier entsteht der Prototyp einer WPF-Blazor Hybrid App mit dynamischer Nachladung von Modulen und nach MVVM Prinzipien

Aktueller Stand: unvollständig 

01_WPF

Stand: Ein "leeres" WPF-C#-Projekt - dort soll der Code rein, der die Blazor-App startet

10_Anwendung-Blazor

Stand: Das ist das aktuelle Blazor-Programm. Erstellt nach MVVM-Prinzipien. Enthält eine "Settings"-Seite, womit man dann Einstellungen je nach Art (Benutzer, Client und Anwendungsweit) einstellen kann

Anmerkung: Die Settings-Seite geht in diesem Projekt leider noch nicht auf, weil angeblich die Referenz auf System.Windows.Forms fehlen würde

Zusätzlich ist noch implementiert, dass weitere Module dynamisch nachgeladen werden und dann im Menübaum auch angezeigt werden

20_Modul - Cars

Stand: Hier handelt es sich um ein simples Beispiel für ein Modul. Es hat keine weiteren Funktionen, sondern dient nur der Anzeige einer weiteren "Funktion" (hier z.B. das Anlegen von neuen Fahrzeugen)

90_Framework

Hier sind die von mir entwickelten Projekte die mein Framework darstellen. Um das Projekt nich zu unübersichtlich zu machen, hab ich die aktuellen DLLs angebunden - zu finden in dem Verzeichnis DLL. Aber anscheinend klappt da was mit dem "Auflösen" der Referenzen nicht, darum muss ich einige Projekte doch hier ablegen
