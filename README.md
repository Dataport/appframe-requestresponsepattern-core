# <p align="center">Request-Response-Pattern</p>

![GitHub](https://img.shields.io/github/license/Chips100/clr2ts.svg)

## Über das Projekt

Request-Response-Pattern bietet als Komponente eine Basis zur Entwicklung nach dem Grundsatz des *Design by Contract*. Die Signatur einer Funktion wird dabei durch Klassen modelliert, sogenannte Requests für Eingabeparameter und Responses für Rückgabewerte. Diese legen neben der reinen Struktur der Daten auch weitere Regeln zu Vor- und Nachbedingungen fest, beispielsweise über Validierungsattribute. Dadurch sind die einzuhaltenden Verträge für die Konsumenten transparent ersichtlich.

Die Implementation der Funktion erfolgt über sogenannte Handler, die einen eingehenden Request verarbeiten und das Ergebnis in einem Response festhalten. 

Die Komponente löst dabei viele typische Problemstellungen, wie z.B.:
* Die Ausgabe von Meldungen zusätzlich zum Ergebnis einer Funktion
* Das Einstreuen von Cross-Cutting-Concerns über aspektorientierte Ansätze
* Das Auflösen eines zuständigen Handlers zur Laufzeit
* Die Unterstützung mehrerer Handler für einen Request


### AppFrameDotNet

Bei RequestResponsePattern handelt es sich um eine Komponente des Frameworks AppFrameDotNet. 

AppFrameDotNet ist eine Sammlung von Bibliotheken und Frameworks, die bei der Erstellung von Fachanwendungen für die öffentliche Verwaltung hilft. Die technische Basis bildet .NET Standard 2.0 sowie .NET Core 3.1. AppFrameDotNet wird seit 2015 bei Dataport AöR für den internen Gebrauch entwickelt, unter der Bezeichnung AppFrameTS gibt es zudem integrative Komponenten um Angular Single Page Applications  in den Stack mit einzubeziehen.

Die Bestandteile werden sukzessive als Open Source veröffentlicht werden. 

## Das Repository

Dieses Repository enthält den gesamten Quellcode sowie die Entwicklungsdokumentation der Komponente RequestResponsePattern. Die Arbeitssprache ist Deutsch. 

### Lizenz

Das Projekt ist unter der MIT-Lizenz lizenziert. Nähere Informationen dazu findet ihr hier: [Lizenzinformationen](https://github.com/Dataport/appframe-dotnettools/blob/main/LICENCE). TODO: Link

### Community und Contribution

Derzeit ist eine Beteiligung durch Dritte noch nicht möglich. Wir arbeiten an den organisatorischen Vorbedingungen dies zukünftig möglich zu machen. 

## Über uns

[Dataport](https://www.dataport.de/) ist Digitalisierungsexperte und Innovationspartner der öffentlichen Verwaltung.  Es ist unsere Aufgabe, digitale Souveränität sicherzustellen und Public Value zu schaffen. Dazu gehört ein verstärktes Engagement, als Teil der Open-Source-Community diese innovativ mitzugestalten.

## Kontakt

Wenn ihr Fragen, Anmerkungen und Anregungen zu AppFrameDotNet oder einzelnen Komponenten habt, meldet euch gerne bei uns. Schreibt dafür einfach eine E-Mail an: [appframe@dataport.de](mailto:appframe@dataport.de).

