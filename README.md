# Binary City Practical Assessment

## Project Overview

This application is an internal management system developed as part of a software developer practical assessment. It demonstrates proficiency in ASP.NET Core MVC, Entity Framework Core, and relational database design through a client and contact management system with many-to-many relationship capabilities.

The system enables users to manage clients and their associated contacts while maintaining bidirectional relationships. A key feature includes automated client code generation following specific business rules.

---

## Technology Stack

- **Framework:** ASP.NET Core MVC
- **Language:** C#
- **ORM:** Entity Framework Core
- **Database:** SQLite

---

## Core Features

### Client Management
- Create, view, and edit client records
- Automatic client code generation upon initial save
- Alphabetically sorted client listings
- Contact count display for each client
- Ability to link and unlink contacts directly from the client form

### Contact Management
- Create, view, and edit contact records
- Email validation with uniqueness enforcement
- Contacts sorted by surname, then first name
- Client count display for each contact
- Ability to link and unlink clients directly from the contact form

### Relationship Management
- Many-to-many relationship implementation between clients and contacts
- Bidirectional linking and unlinking capabilities
- Relationship integrity maintained through Entity Framework Core

---

## Client Code Generation Logic

Client codes follow a standardized format: **AAA001**

### Business Rules:
1. The first three characters are derived from the client's name
2. Characters are converted to uppercase
3. The numeric suffix begins at 001
4. The number increments sequentially for each client sharing the same three-letter prefix
5. Code generation occurs only after the initial database save to ensure uniqueness

### Example:
- **Client Name:** Binary City  
  **Generated Code:** BIN001
- **Client Name:** Binary Solutions  
  **Generated Code:** BIN002

---

## Installation and Setup

### Prerequisites
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download) or higher
- A code editor (Visual Studio Code or Visual Studio recommended)
- Git (for cloning the repository)

### Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone https://github.com/YOUR_USERNAME/BinaryCity.Practical.git
   cd BinaryCity.Practical
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

5. **Access the application:**  
   Open your browser and navigate to the URL displayed in the terminal (typically `https://localhost:5001` or `http://localhost:5000`)

---

## Project Architecture

```
BinaryCity.Practical/
│
├── Controllers/          # MVC Controllers for handling requests
├── Models/              # Domain models and view models
├── Views/               # Razor views for UI rendering
├── Data/                # DbContext and database configuration
├── wwwroot/             # Static files (CSS, JavaScript, images)
├── Migrations/          # Entity Framework migrations
└── README.md
```

---

## Technical Implementation

### Database Design
- Implemented a many-to-many relationship using Entity Framework Core navigation properties
- Utilized SQLite for lightweight data persistence
- Applied appropriate indexing for email uniqueness constraints

### Validation Strategy
- Server-side validation for required fields
- Email format validation using data annotations
- Custom validation logic for client code uniqueness
- Database-level constraints for data integrity

### MVC Architecture
- Separation of concerns maintained throughout the application
- Controller actions handle business logic and data operations
- Views provide clean, functional user interfaces
- Models represent domain entities and validation rules

---

## Key Development Decisions

### 1. Client Code Generation Approach
To ensure unique and correctly formatted client codes, I implemented a two-phase save process:
- Phase 1: Save the client entity to generate a database ID
- Phase 2: Calculate the appropriate prefix and numeric suffix based on existing codes
- Phase 3: Update the client record with the generated code

This approach guarantees code uniqueness while adhering to the specified business rules.

### 2. Many-to-Many Relationship Management
Entity Framework Core's built-in many-to-many navigation properties were utilized to simplify relationship management. Controller actions facilitate adding and removing relationships from either the client or contact perspective, ensuring data consistency.

### 3. Email Uniqueness Implementation
A multi-layered approach ensures email uniqueness:
- Model-level validation attributes
- Database unique index constraint
- Server-side validation with user-friendly error messaging

---

## Testing the Application

### Sample Data Creation
To verify functionality:
1. Navigate to the Clients page and create at least two clients
2. Navigate to the Contacts page and create at least two contacts
3. Link contacts to clients using the management interface
4. Verify that relationships appear correctly on both sides
5. Test unlinking functionality

---

## Future Enhancements

Given additional development time, the following improvements could be implemented:

**Feature Enhancements:**
- Search and filtering capabilities for client and contact lists
- Pagination for improved performance with large datasets
- Audit logging for tracking changes to records
- Export functionality (CSV, Excel)

**Technical Improvements:**
- Comprehensive unit test coverage
- Integration testing for database operations
- Service layer abstraction for business logic
- Repository pattern implementation
- Enhanced error handling and logging

**User Experience:**
- Responsive design for mobile devices
- Advanced UI styling and accessibility improvements
- Inline editing capabilities
- Bulk operations support

---

## Challenges and Solutions

### Challenge 1: Client Code Generation Timing
**Issue:** Client codes needed to be unique and follow specific formatting rules, but uniqueness could only be guaranteed after database persistence.

**Solution:** Implemented a deferred generation strategy where codes are assigned after the initial save operation, allowing the system to query existing codes and determine the next available number in the sequence.

### Challenge 2: Bidirectional Relationship Management
**Issue:** Managing many-to-many relationships from both client and contact perspectives while maintaining data consistency.

**Solution:** Leveraged Entity Framework Core's navigation properties and change tracking to automatically handle relationship synchronization, reducing complexity and potential for errors.

### Challenge 3: Email Validation
**Issue:** Ensuring email uniqueness across the system while providing clear feedback to users.

**Solution:** Combined model validation, database constraints, and explicit server-side checks to create a robust validation pipeline with informative error messages.

---

## Development Environment

- **IDE:** Visual Studio Code / Visual Studio 2022
- **Version Control:** Git
- **Database Management:** DB Browser for SQLite (for development/debugging)

---

## Notes

This project was developed as part of a practical assessment to demonstrate competency in:
- ASP.NET Core MVC architecture
- Entity Framework Core for data access
- Relational database design
- Business logic implementation
- Input validation and data integrity
- Clean code practices

---

## Author

**Nicolette Mashaba**  
Aspiring Software Developer

---

## License

This project is submitted as part of a practical assessment and is intended for evaluation purposes.