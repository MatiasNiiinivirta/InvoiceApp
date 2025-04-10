LaskuApp
LaskuApp is a WPF (Windows Presentation Foundation) application that allows users to manage invoices (Lasku) and products (Tuote) in a database. The application provides features such as searching invoices, adding new invoices and products, updating existing invoices, and deleting invoices. This is ideal for businesses that have a steady product inventory and offer services. It's well-suited for situations where invoicing is needed for both the work done and the products purchased for the job. Examples include construction companies, maintenance services, and fire damage restoration. The UI language is in Finnish.

![Näyttökuva 2025-04-10 151353](https://github.com/user-attachments/assets/9f9d263e-5396-4d81-af62-94538762e1c4)


Features

Add new invoice ( Lisää uusi lasku )
The main function of the app is to generate invoices. You can add products to the invoice as long as they are available in the inventory by clicking 'Add New Row' (Lisää uusi rivi). The invoice ID (Laskun numero) is automatically generated in the backend. The total amount (Kokonaissumma) is calculated automatically based on the number of products listed in the rows, their prices, as well as the work hours (Työaika/h) and salary (Palkka/h).

![Näyttökuva 2025-04-10 151905](https://github.com/user-attachments/assets/b285d034-b67a-4485-941d-63f4a69c6916)

Add new product
You can also add new products to the inventory, which is stored in the database. Additionally, you can search for existing products, update their names or prices, or remove them entirely.
![Näyttökuva 2025-04-10 152330](https://github.com/user-attachments/assets/2d50dc5e-96fe-4b3a-af20-6efb5ed6d594)

Update selected invoice (Päivitä valittu lasku )
All invoices are stored in the database, and you can search for and select them to be updated or deleted. When you choose to modify an invoice, the app opens a view similar to the one for creating a new invoice, but with all the existing information for the selected invoice pre-loaded from the database. The app will update the information based on the invoice ID when you click 'Save' (Tallenna).
![Näyttökuva 2025-04-10 152700](https://github.com/user-attachments/assets/a1182266-9aa1-440c-8515-c6f5905baf64)

The app uses MariaDB as its database, which was hosted on my personal computer. However, in this version available on GitHub, there is no active database for security reasons. This is also why the app does not have a built .exe file. You can, however, link your own database to the app via the LaskuRepo.cs file.
![5-04-10 153043](https://github.com/user-attachments/assets/da43f1c8-646f-4c1d-9cb2-48e003472c8a)

Technical overview
C#,
XAML,
.NET (6.0)

Build in 
Visual studio

Database MariaDb

How to run:
Download the ZIP file from GitHub.

Extract the contents of the ZIP file.

Open the solution file (.sln) in Visual Studio.

Modify the database connection settings to match your environment.

Build and run the project in Visual Studio
