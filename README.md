Customer Services Portal - Technical Examination <br>
📌 Project Overview<br>
A web-based portal developed in .NET 8 MVC designed to streamline customer service requests. Clients can manage acquired assets, submit support tickets with document attachments, and track the entire lifecycle of their requests through an interactive timeline.

🛠 Tech Stack & Architecture

Framework: .NET 8 ASP.NET Core MVC 


Database: MS SQL Server with Required Stored Procedures 


Frontend: Bootstrap 5 & JavaScript 


Pattern: 3-Layer Architecture (Web, Service, and Data Layers) 


Design Principles: Dependency Injection (DI), OOP, and Asynchronous Programming 

🚀 Key Technical Features

Stored Procedures: Implemented for ticket creation, status updates, and automated timeline logging to ensure data integrity.


Parallel Programming: Utilized Parallel.ForEach and ConcurrentDictionary on the Admin Dashboard to process ticket statistics efficiently across multiple cores.


Document Management: Integrated file upload system for document requests (e.g., contracts, clearances).


Security: Role-based authorization (Admin vs. Client) and robust input validation (Client-side & Server-side).

⚙️ Setup Instructions
Database: * Locate the /Database folder in the root directory.

Execute the SQL script (or restore the .bak file) in MS SQL Server to generate tables and stored procedures.

Configuration:

Update appsettings.json with your local connection string.

Current setting: Data Source=localhost;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Database=CustomerServicesPortal

Run:

Open the solution in Visual Studio 2022.

Press F5 to build and run.
