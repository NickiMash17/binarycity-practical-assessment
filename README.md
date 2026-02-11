# Binary City Practical Assessment

## Overview
This project is a small internal management system built as part of a software developer practical assessment.  
The application allows users to manage clients and contacts, and maintain many-to-many relationships between them.

Each client can have multiple contacts, and each contact can be linked to multiple clients.  
The system also generates a unique client code based on the client’s name, following the required business rules.

---

## Tech Stack
- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQLite

---

## Features
### Clients
- Create and edit clients
- Automatic client code generation after first save
- Clients listed alphabetically
- Display number of linked contacts
- Link and unlink contacts from the client form

### Contacts
- Create and edit contacts
- Email validation and uniqueness enforcement
- Contacts listed by surname and name
- Display number of linked clients
- Link and unlink clients from the contact form

### Relationship
- Many-to-many relationship between clients and contacts
- Managed using Entity Framework Core

---

## Client Code Generation
Client codes follow the format:

AAA001

Rules:
- First three letters derived from the client name
- Converted to uppercase
- Numeric portion starts at 001
- Increments for each client with the same prefix
- Generated only after the client is saved

---

## How to Run the Project

### Prerequisites
- .NET SDK installed
- VS Code or Visual Studio

### Steps
1. Clone the repository:


git clone https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git


2. Navigate to the project folder:


cd BinaryCity.Practical


3. Apply migrations and create the database:


dotnet ef database update


4. Run the application:


dotnet run


5. Open the browser at the URL shown in the terminal.

---

## Project Structure


BinaryCity.Practical/
│
├── Controllers/
├── Models/
├── Views/
├── Data/
├── wwwroot/
└── README.md


---

## Key Design Decisions
- Used ASP.NET Core MVC to follow the requested MVC architecture.
- Implemented a many-to-many relationship using EF Core navigation properties.
- Added server-side validation for required fields and unique email addresses.
- Kept the UI simple and functional, focusing on correctness and clarity.

---

## Challenges and Solutions

### 1. Client code generation logic
**Challenge:**  
The client code had to follow strict rules and only be generated after the client was saved.

**Solution:**  
I implemented a two-step process:
1. Save the client with the name.
2. Generate the prefix and determine the next available number.
3. Update the client with the final code.

This ensures the code is always unique and follows the required format.

---

### 2. Many-to-many relationship handling
**Challenge:**  
Clients and contacts needed to be linked and unlinked from both sides.

**Solution:**  
I used EF Core’s many-to-many navigation properties and created controller actions to add or remove relationships. This kept the logic simple and aligned with the MVC structure.

---

### 3. Email uniqueness validation
**Challenge:**  
The system needed to prevent duplicate contact emails.

**Solution:**  
I implemented:
- Model validation for required email fields
- A unique index in the database
- Server-side checks to show clear error messages

---

## Possible Improvements
If more time were available, I would:
- Add search and filtering to list pages
- Improve UI styling and responsiveness
- Add unit tests for business logic
- Refactor controller logic into service classes
- Add pagination for large datasets

---

## Author
Nicolette Mashaba  
Graduate Software Engineer

Final tip

Before submitting:

Run the project once.

Create:

2 clients

2 contacts

Link them

Take one screenshot of the working app.

Add it to a /screenshots folder.

Mention it in the README.

That makes your repo look much more professional.
