Product Management System
This project is a .NET Core MVC web application that demonstrates a simple management system for products, featuring CRUD operations, security measures, data encryption, and the use of Entity Framework Core for database interactions.

Project Overview
The Product Management System is designed to manage products with functionalities to create, read, update, and delete (CRUD) product entries. This system incorporates security measures such as authentication and authorization to ensure that only authorized users can perform certain actions. Sensitive data is encrypted to protect confidentiality.

Features
1. CRUD Operations
Create: Add new products to the system.
Read: View a list of all products and details of each product.
Update: Modify details of existing products.
Delete: Remove products from the system (restricted to admin users only).
2. Security Measures
Authentication: Users must log in to access the product management system.
Authorization: Role-based access control ensures that:
Admin users can perform all CRUD operations.
Regular users can view and update products but cannot delete them.
3. Data Encryption
Password Encryption: User passwords are hashed before being saved to the database.
Decryption: During login, the hashed passwords are compared to authenticate users.
4. Entity Framework Core
Database Interactions: Entity Framework Core is used to handle all database operations.
Data Models: Appropriate data models are defined and configured.
Migrations: Migrations are used to create and update the database schema.
Technical Details
Technologies Used
ASP.NET Core MVC: The main framework used for building the web application.
Entity Framework Core: Used for database operations.
SQL Server: The database system used for storing data.
ASP.NET Core Identity: Used for managing user authentication and authorization.
Hashing Algorithms: Industry-standard algorithms are used for password hashing.

Setup Instructions

Copy code
dotnet ef migrations add MigrationName
Update the database:

bash
Copy code
dotnet ef database update
Usage
Register a new user: Users can register by providing their details.
Login: Registered users must log in to access the product management system.
Role-Based Access:
Admin: Can create, read, update, and delete products.
User: Can create, read, and update products but cannot delete them.
Conclusion
This project showcases a simple yet comprehensive product management system built with .NET Core MVC. It covers essential functionalities such as CRUD operations, security measures, data encryption, and the use of Entity Framework Core for efficient database management.

Feel free to explore and enhance the project further. Contributions are welcome!