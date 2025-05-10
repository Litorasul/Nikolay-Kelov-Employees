# Nikolay-Kelov-Employees
This is a full-stack web application built with **ASP.NET Core** and **Angular** that allows you to upload a CSV file and calculate how long each pair of employees has worked together on the same project.

![image](https://github.com/user-attachments/assets/3f5cff26-0ece-4318-adee-581da20526b4)

## 🧠 Features

- Upload a CSV file containing employee project history.
- Parse and validate different date formats.
- Calculate overlap durations between employee pairs on the same project.
- Display results in a user-friendly table.
- Includes unit tests (xUnit) for backend logic.

---

## 📁 CSV File Format

Your CSV file should be structured like this:

- `EmpID`: Employee ID (integer)
- `ProjectID`: Project ID (integer)
- `DateFrom`: Start date (ISO format or other supported formats)
- `DateTo`: End date or `NULL` (indicating "to present")

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Node.js & npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)

### Backend (ASP.NET Core)

```bash
cd EmployeesOverlap
dotnet restore
dotnet run
```
### Frontend (Angular)

```bash
cd EmployeesOverlap/EmployeesOverlap.Client
npm install
npm start
```
## 🏗️ Architecture

This project follows a clean, modular architecture using **.NET 9 (ASP.NET Core)** for the backend and **Angular** for the frontend.

### 🧩 Key Components

- **CsvParser**: Parses uploaded CSV files into structured models.
- **OverlapCalculator**: Calculates pairwise employee project overlaps.
- **EmployeeOverlapService**: Orchestrates parsing and overlap calculation.
- **EmployeeController**: Exposes the API endpoint for CSV uploads.
- **Angular App**: Handles file upload and renders results in a clean UI.

### 🧪 Testing

- Unit tests are written using **xUnit** for backend services.
- Each service (parser, calculator, orchestrator) has comprehensive coverage.

### 🔁 Communication Flow

1. User uploads CSV via Angular frontend.
2. Angular sends the file to `/employee/overlaps` via HTTP POST.
3. ASP.NET Core backend parses the file, calculates overlaps, and returns results.
4. Angular renders the results in a responsive table.

