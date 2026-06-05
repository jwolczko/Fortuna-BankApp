# Aplikacja bankowa ***Fortuna***

Aplikacja bankowa na zaliczenie przedmiotu "Projektowanie wielowarstwowych aplikacji biznesowych"

## Wymagane narzędzia:

 * [Visual Studio 2026](https://visualstudio.microsoft.com/pl/thank-you-downloading-visual-studio/?sku=Community&channel=Stable&version=VS18&source=VSLandingPage&cid=2500&passive=false) lub [Ridder](https://www.jetbrains.com/rider/download/)
 * [Visual Studio Code](https://code.visualstudio.com/Download)
 * [SQL Server Management Studio](https://aka.ms/ssms/22/release/vs_SSMS.exe) lub inne narzędzie umożliwiające łączenie się z MS SQL Server
 * [Bruno](https://www.usebruno.com/downloads)
 * [Git](https://git-scm.com)


## Uruchomienie projektu:

### Frontend:

Wymagana jest instalacja NodeJs w wersji 25.8.1 lub nowszej:
[Obecna werjsa do pobrania](https://nodejs.org/en/download/current)

Po zainstalowaniu nodejs należy się upewnić że ścieżka została prawidłowo dodana do zmiennej środowiskowej **PATH**.
Można to zrobić po przez uruchomienie wiersz poleceń lub PowerShella i wpisanie : **npm --version** lub **node --version**.
Jeżeli zostanie zwrócona wersja to znaczy że jest ok.

W celu uruchomienia projektu należy w Visual Studio Code należy:
* kliknąć w menu *File* -> *Open Folder...*
* następnie wybrać folder: *..\Designing-multi-tier-business-applications\frontend*
* po otworzeniu projektu należy kliknąć w menu *Terminal* -> *New Teminal*
* w teminalu należy wpisać: **npm install**
* po zainstalowaniu wrzystkich pakietów można uruchomić projekt wpisując: **npm run dev** 


### Backend:
Do pełnego działania backendu wymagamane jest poprawne zainstalowanie Visula Studio 2026
oraz .NET 10.
W celu uruiichomienia projektu należy za pomocą Visula Studio lub Riddera uruchomić plik: **Fortuna.slnx** z folderu : *..\Designing-multi-tier-business-applications\backend*

Backend można też uruchomić bezpośrednio z terminala. Z katalogu głównego repozytorium należy wykonać:

```powershell
cd backend/src/Fortuna.Api
dotnet restore
dotnet run
```

Po uruchomieniu aplikacji terminal wyświetli adres API, np. `https://localhost:xxxx` albo `http://localhost:xxxx`.
Swagger jest dostępny pod adresem:

```text
https://localhost:xxxx/swagger
```

albo:

```text
http://localhost:xxxx/swagger
```

Poprawność działania API można sprawdzić endpointem:

```text
GET /api/health
```

Przed uruchomieniem backendu należy upewnić się, że SQL Server działa oraz że zostały wykonane skrypty tworzące bazy `FortunaWriteDb` i `FortunaReadDb`.

### Bazy danych:
Do baz danych należy zainstalować [**SQL Serve 2025 Developer**](https://go.microsoft.com/fwlink/?linkid=2344626&clcid=0x409&culture=en-us&country=us). Po poprawnym zainstalowaniu instancji SQL Servera należy uruchomić SQL Server Management Studio.
Następnie należy kliknąż w menu *Plik* -> *Otwórz* -> *Projekt/rozwiązanie*,
jak pojawi się okno dialogowe z wyborem projektu należy wskazać plik **FortunaDb.slnx**
z folderu *..\Designing-multi-tier-business-applications\database*.
Po uwuchomieniu projektu należy wykonać 2 skrypty: **FortunaReadDb.sql** i **FortunaWriteDb.sql** które odtworzą bazy danych.

