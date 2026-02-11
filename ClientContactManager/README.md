# Client Contact Manager

A web application for managing clients and contacts with many-to-many relationship support. Built with ASP.NET Core MVC and Entity Framework Core using SQLite database.

## Features

- **Contact Management**
  - Create, read, update, and delete contacts
  - Email validation and uniqueness enforcement
  - Track number of linked clients per contact
  - Sort contacts by full name (Surname Name) ascending

- **Client Management**
  - Create, read, update, and delete clients
  - Auto-generated unique client codes (format: AAA001)
  - Client codes are immutable after creation
  - Track number of linked contacts per client
  - Sort clients by name ascending

- **Many-to-Many Linking**
  - Link contacts to multiple clients
  - Link clients to multiple contacts
  - Bidirectional linking from both contact and client edit forms
  - Easy unlinking functionality
  - Duplicate link prevention

- **User Interface**
  - Responsive Bootstrap design
  - Tabbed interface for edit forms (General + Linked entities)
  - Real-time validation with clear error messages
  - Success/error feedback messages
  - Clean, professional layout

## Technologies Used

- **ASP.NET Core 10.0** - MVC framework
- **Entity Framework Core 10.0** - ORM
- **SQLite** - Database
- **Bootstrap 5** - UI framework
- **jQuery** - Client-side validation

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio Code](https://code.visualstudio.com/) with C# extension
- [DB Browser for SQLite](https://sqlitebrowser.org/) (optional, for viewing database)

## Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/<your-username>/binarycity-practical-assessment.git
cd binarycity-practical-assessment/ClientContactManager
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Create Database

```bash
# Create initial migration (already included, but if needed):
dotnet ef migrations add InitialCreate

# Apply migration to create database
dotnet ef database update
```

This will create a `clientcontactmanager.db` SQLite database file in the project root.

### 4. Run the Application

```bash
dotnet run
```

Or for hot reload during development:

```bash
dotnet watch run
```

The application URL is defined in `Properties/launchSettings.json` (for example `https://localhost:7041` and `http://localhost:5078` by default).

## Project Structure

```
ClientContactManager/
├── Controllers/
│   ├── HomeController.cs          # Home page controller
│   ├── ContactsController.cs      # Contact CRUD operations
│   └── ClientsController.cs       # Client CRUD operations
├── Data/
│   └── ApplicationDbContext.cs    # EF Core database context
├── Models/
│   ├── Contact.cs                 # Contact entity
│   ├── Client.cs                  # Client entity
│   ├── ClientContact.cs           # Join entity for many-to-many
│   └── ErrorViewModel.cs          # Error handling
├── Services/
│   └── ClientCodeGenerator.cs     # Client code generation logic
├── Views/
│   ├── Home/                      # Home page views
│   ├── Contacts/                  # Contact views (Index, Create, Edit, Delete)
│   ├── Clients/                   # Client views (Index, Create, Edit, Delete)
│   └── Shared/                    # Shared layouts and partials
├── wwwroot/
│   ├── css/                       # Stylesheets
│   └── js/                        # JavaScript files
├── appsettings.json               # Configuration
├── Program.cs                     # Application entry point
└── ClientContactManager.csproj    # Project file
```

## Database Schema

### Tables

**Contacts**
- Id (Primary Key)
- Name
- Surname
- Email (Unique)

**Clients**
- Id (Primary Key)
- Name
- ClientCode (Unique, 6 characters: AAA001)

**ClientContacts** (Join Table)
- ClientId (Foreign Key)
- ContactId (Foreign Key)
- Composite Primary Key: (ClientId, ContactId)

### Relationships

- One Contact can be linked to many Clients
- One Client can be linked to many Contacts
- Many-to-Many relationship through ClientContacts join table

## Client Code Generation Logic

Client codes are automatically generated when a new client is created:

1. **Extract Prefix**: Take the first 3 letters from the client name, convert to uppercase
   - If name has fewer than 3 letters, pad with 'A'
   - If name has no letters, default to "AAA"

2. **Find Next Number**: Query database for existing codes with the same prefix
   - Find the lowest available number (001-999)
   - Numbers are assigned sequentially

3. **Examples**:
   - "First National Bank" → FIR001
   - "Protea Holdings" → PRO001
   - "AB Corp" → ABA001
   - If FIR001 exists, next is FIR002

## Usage Examples

### Creating a Contact

1. Navigate to **Contacts** → **Create New Contact**
2. Fill in:
   - Name (required)
   - Surname (required)
   - Email (required, must be unique, valid email format)
3. Click **Create**

### Creating a Client

1. Navigate to **Clients** → **Create New Client**
2. Fill in:
   - Name (required)
3. Click **Create**
4. Client code is automatically generated and displayed

### Linking Contact to Client

**From Contact Edit Page:**
1. Navigate to **Contacts** → Click **Edit** on a contact
2. Go to **Client(s)** tab
3. Select a client from the dropdown
4. Click **Link Client**

**From Client Edit Page:**
1. Navigate to **Clients** → Click **Edit** on a client
2. Go to **Contact(s)** tab
3. Select a contact from the dropdown
4. Click **Link Contact**

### Unlinking

From either the Contact's Client(s) tab or Client's Contact(s) tab:
1. Find the linked entity in the table
2. Click the **Unlink** link in the Action column
3. Confirm the action

## Validation Rules

### Contact
- **Name**: Required
- **Surname**: Required
- **Email**: Required, must be valid email format, must be unique across all contacts

### Client
- **Name**: Required
- **Client Code**: Auto-generated, 6 characters (3 letters + 3 digits), unique, immutable after creation

### Linking
- Cannot link the same contact to the same client more than once
- Duplicate link attempts show an error message

## Development Commands

### Entity Framework Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName

# Update database to latest migration
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script

# Drop database
dotnet ef database drop
```

### Build & Run

```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Run with hot reload
dotnet watch run

# Clean build artifacts
dotnet clean
```

## Testing Checklist

- [ ] Create contacts with all fields
- [ ] Test email validation (invalid format)
- [ ] Test email uniqueness (duplicate email)
- [ ] Create clients and verify auto-generated codes
- [ ] Test client code formats for different names
- [ ] Edit contact information
- [ ] Edit client information (code remains unchanged)
- [ ] Link contact to multiple clients
- [ ] Link client to multiple contacts
- [ ] Unlink from both contact and client sides
- [ ] Verify counts update correctly
- [ ] Test empty states display correctly
- [ ] Delete contacts and clients
- [ ] Test responsive design on mobile

## Known Limitations

- Maximum 999 clients per prefix (e.g., FIR001-FIR999)
- Client codes cannot be changed after creation
- No soft delete functionality (entities are permanently deleted)
- No audit logging (created/modified timestamps)

## Future Enhancements

- Search and filter functionality
- Pagination for large datasets
- Export to CSV/Excel
- Bulk operations
- Audit logging with timestamps
- Soft delete with restoration
- User authentication and authorization
- Advanced reporting and analytics

## Troubleshooting

### Database Issues

If you encounter database errors:

```bash
# Delete the database file
rm clientcontactmanager.db

# Recreate from migrations
dotnet ef database update
```

### Migration Issues

If migrations fail:

```bash
# Remove Migrations folder
rm -rf Migrations/

# Create fresh migration
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Port Already in Use

If the configured port is already in use, edit `Properties/launchSettings.json` to use different ports.

## Contributing

This is a practical assessment project. For educational purposes only.

## License

This project is part of a practical assessment for BinaryCity.

## Note

This application was developed as part of a practical assessment to demonstrate ASP.NET Core MVC, Entity Framework Core, and web development best practices.
