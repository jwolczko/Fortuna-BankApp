# ***Fortuna*** Banking Application

A banking application created as a course project for "Designing Multi-Tier Business Applications".

## Required Tools

* [Visual Studio 2026](https://visualstudio.microsoft.com/pl/thank-you-downloading-visual-studio/?sku=Community&channel=Stable&version=VS18&source=VSLandingPage&cid=2500&passive=false) or [Rider](https://www.jetbrains.com/rider/download/)
* [Visual Studio Code](https://code.visualstudio.com/Download)
* [SQL Server Management Studio](https://aka.ms/ssms/22/release/vs_SSMS.exe) or another tool that can connect to MS SQL Server
* [Bruno](https://www.usebruno.com/downloads)
* [Git](https://git-scm.com)

## Running the Project

### Frontend

Node.js version 25.8.1 or newer is required:
[Download the current version](https://nodejs.org/en/download/current)

After installing Node.js, make sure that the installation path has been correctly added to the **PATH** environment variable.
You can check this by opening Command Prompt or PowerShell and running: **npm --version** or **node --version**.
If a version number is returned, everything is configured correctly.

To run the project in Visual Studio Code:

* click *File* -> *Open Folder...*
* select the folder: *..\Designing-multi-tier-business-applications\frontend*
* after opening the project, click *Terminal* -> *New Terminal*
* in the terminal, run: **npm install**
* after all packages have been installed, start the project with: **npm run dev**

### Backend

For the backend to work correctly, Visual Studio 2026 and .NET 10 must be properly installed.
To run the project, open the **Fortuna.slnx** file from the *..\Designing-multi-tier-business-applications\backend* folder using Visual Studio or Rider.

The backend can also be started directly from the terminal. From the repository root directory, run:

```powershell
cd backend/src/Fortuna.Api
dotnet restore
dotnet run
```

After the application starts, the terminal will display the API address, for example `https://localhost:xxxx` or `http://localhost:xxxx`.
Swagger is available at:

```text
https://localhost:xxxx/swagger
```

or:

```text
http://localhost:xxxx/swagger
```

You can verify that the API is working by calling the following endpoint:

```text
GET /api/health
```

Before starting the backend, make sure that SQL Server is running and that the scripts creating the `FortunaWriteDb` and `FortunaReadDb` databases have been executed.

### Databases

For the databases, install [**SQL Server 2025 Developer**](https://go.microsoft.com/fwlink/?linkid=2344626&clcid=0x409&culture=en-us&country=us). After correctly installing the SQL Server instance, start SQL Server Management Studio.
Then click *File* -> *Open* -> *Project/Solution*.
When the project selection dialog appears, select the **FortunaDb.slnx** file from the *..\Designing-multi-tier-business-applications\database* folder.
After opening the project, run the two scripts: **FortunaReadDb.sql** and **FortunaWriteDb.sql**, which will recreate the databases.
