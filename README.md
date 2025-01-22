LaskuApp
LaskuApp is a WPF (Windows Presentation Foundation) application that allows users to manage invoices (Lasku) and products (Tuote) in a database. The application provides features such as searching invoices, adding new invoices and products, updating existing invoices, and deleting invoices.

Features
Database and Tables Creation: The application automatically creates a database and the necessary tables for storing invoices and products.
Search Functionality: Users can search for invoices by invoice number or customer name. The search results are dynamically displayed in a popup.
Add New Invoice: Users can add a new invoice through the "LuoLasku" view.
Add New Product: Users can add new products through the "LuoTuote" view.
Update Invoice: Users can select an invoice to update, and the corresponding update view is displayed.
Delete Invoice: Users can delete an invoice from the database.
Mouse Interaction: The app provides dynamic mouse interaction for buttons and hiding search results when clicking on the canvas.
Getting Started
Prerequisites
.NET Framework 4.7 or higher: Ensure that you have the appropriate version of the .NET Framework installed.
Database Access: The application uses a local database for storing invoices and products, so you need a valid SQL Server or SQLite connection for the database operations.
Running the Application
Clone the repository or download the project files.
Open the project in Visual Studio or your preferred IDE.
Build and run the project.
Usage
Search Invoices: Enter the invoice number or customer name in the search box. The search results will be displayed in the popup, and you can select an invoice to update or delete.
Add New Invoice: Click on the "LuoLasku" button to open the view for adding a new invoice.
Add New Product: Click on the "LuoTuote" button to open the view for adding a new product.
Update Invoice: If you have selected a matching invoice from the search, click the "Update Invoice" button to modify the invoice details.
Delete Invoice: Click the "Remove Invoice" button to delete the selected invoice.
Exit: Close the application by clicking the "Exit" button.
Code Structure
MainWindow.xaml.cs: The main window logic for managing invoices and products, including database operations, search functionality, and button interactions.
LaskuRepo: A repository class responsible for interacting with the database, including methods for creating tables, adding default data, and performing CRUD operations on invoices and products.
Lasku: Represents an invoice with properties like invoice number and customer name.
Tuote: Represents a product with properties like product name and price.
License
This project is open source and is licensed under the MIT License.

Acknowledgements
Thanks to the contributors and the community for their ongoing support and feedback.
