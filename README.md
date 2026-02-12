# Binary City Practical Assessment

## Overview
This is a practical assessment project built with ASP.NET Core MVC.

The app manages:
- clients
- contacts
- many-to-many links between clients and contacts

It also generates a unique client code in `AAA001` format based on the client name.

## Tech Stack
- ASP.NET Core MVC (.NET 10)
- C#
- Entity Framework Core
- SQLite

## What Works
- Create, edit, and delete clients
- Create, edit, and delete contacts
- Link/unlink contacts to clients from both sides
- Contact email uniqueness validation
- Auto-generated client codes

## Client Code Generation
Code format: `AAA001`

Rules implemented:
- First 3 letters from the client name (letters only, uppercase)
- Pad with `A` if fewer than 3 letters
- Suffix starts at `001`
- Suffix increments for existing clients with the same prefix

Example:
- `Binary City` -> `BIN001`
- `Binary Solutions` -> `BIN002`

## Run Locally
1. Clone:
```bash
git clone https://github.com/NickiMash17/binarycity-practical-assessment.git
cd binarycity-practical-assessment
```
2. Enter project:
```bash
cd ClientContactManager
```
3. Apply migrations:
```bash
dotnet ef database update
```
4. Run:
```bash
dotnet run
```

## Project Structure
```text
binarycity-practical-assessment/
├── ClientContactManager/
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   ├── Services/
│   ├── Views/
│   └── wwwroot/
├── binarycity-practical-assessment.sln
└── README.md
```

## Notes
- The solution currently has no automated test project.
- Validation was done with manual UI checks (create/edit/link/unlink flows).

## Author
Nicolette Mashaba
