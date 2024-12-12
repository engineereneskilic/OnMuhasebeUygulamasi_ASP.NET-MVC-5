# Pre-Accounting Automation
![**Pre-Accounting Automation (Image)**](pre-account-automation2.jpg)  

## Table of Contents
- [Project Overview](#project-overview)
- [Features](#features)
  - [Extensive Database and Feature Set](#extensive-database-and-feature-set)
  - [Core Accounting Functions](#core-accounting-functions)
  - [Inventory and Customer Management](#inventory-and-customer-management)
  - [Reporting and Analysis](#reporting-and-analysis)
  - [User Roles and Security](#user-roles-and-security)
  - [User Interface and Menus](#user-interface-and-menus)
- [Technologies Used](#technologies-used)
- [Setup Instructions](#setup-instructions)
  - [Prerequisites](#prerequisites)
  - [Installation Steps](#installation-steps)
  - [Database Setup](#database-setup)
- [Usage](#usage)
  - [Example Usage](#example-usage)
- [Future Improvements](#future-improvements)
- [License](#license)

## Project Overview
The Pre-Accounting Automation project is a comprehensive software solution designed to streamline pre-accounting tasks, automate financial record-keeping, and provide critical financial insights. Developed using ASP.NET MVC 5 and C# Forms App (.NET Framework), the application features a robust database structure, extensive functionalities, and various reporting capabilities.

## Features

### 1. Extensive Database and Feature Set
The system utilizes a database with **over 20+ tables** designed to handle various financial and business operations. This includes support for tracking transactions, customer data, purchase orders, inventory, and more. The application provides several feature windows to manage and view essential data.

### 2. Core Accounting Functions
- **Income and Expense Tracking**: Tracks all income and expenses with real-time updates, helping users stay on top of their financial activities.
- **Invoice Generation**: Automatically generates invoices for sales and purchases, ensuring accurate and timely billing.
- **Payment Monitoring**: Monitors payments, including due dates, amounts, and outstanding balances, helping users manage cash flow efficiently.

### 3. Inventory and Customer Management
- **Stock Tracking**: Manage and monitor inventory levels to prevent shortages or excess stock.
- **Purchase Orders**: Create and manage purchase orders with suppliers to ensure timely procurement of goods.
- **Customer Information**: Maintain a comprehensive database of customer details, including order history and contact information.

### 4. Reporting and Analysis
Financial reporting is a key component of the application. It includes:
- **Income Statements**: Automatically generate detailed income statements to track revenues and expenses.
- **Cash Flow Reports**: Analyze cash inflows and outflows to assess financial health.
- **Performance Reports**: Generate reports to evaluate the performance of various financial activities over time.

### 5. User Roles and Security
A role-based authorization system is implemented to ensure that only authorized users can access certain features. The system allows administrators to assign specific tasks and permissions to users, preventing unauthorized access and enhancing security.

### 6. User Interface and Menus
The user interface is designed for intuitive navigation with the following menus and features:

#### Top Horizontal Menu:
- **DOSYA**
- **KARTLAR**
- **İŞLEMLER**
- **ARAÇLAR**
- **HAKKIMIZDA**

#### Left Vertical Menu:
- **Stoklar**
- **Kasalar**
- **Cari Kartlar**
- **Faturalar**
- **Stok Hareketleri**
- **Siparişler**
- **Dekontlar**
- **Çek Senetleri**
- **Bankalar**
- **Adres Defterim**
- **Görevler**

#### Secondary Horizontal Menu (Below Top Menu):
- **Kur Tablosu**
- **Sorgular**
- **Excele Aktar**

#### Bottom Horizontal Menu:
- **Satış Faturası**
- **Alış Faturası**
- **Ödeme**
- **Tahsil**
- **Açılış Kartı**
- **Borç Dekontu**

## Technologies Used
- **Programming Languages**: C#, SQL
- **Frameworks**: ASP.NET MVC 5, .NET Framework
- **Database**: SQL Server (20+ Tables)
- **Reporting**: Custom Reports and Data Analysis
- **Tools & Libraries**:
  - Entity Framework
  - ADO.NET for database communication
  - Role-based security system
  - Custom reporting functions for financial data

## Setup Instructions

### Prerequisites
Before setting up the project, make sure you have the following installed:
- Visual Studio with ASP.NET MVC 5 support
- SQL Server (or an equivalent RDBMS)

### Installation Steps
1. Clone the repository or download the project files.
2. Open the project in Visual Studio.
3. Ensure that the correct SQL Server connection string is set in the web.config file to point to your local or remote database instance.
4. Build the solution and run the application.

### Database Setup
- **Database Creation**: Create a database and restore the provided SQL Scripts to populate the tables.
- **Table Structure**: The database includes 27 tables that store all the required information for accounting, inventory, and customer management.

## Usage
Once the application is running, users can interact with the system through a simple and intuitive UI. The primary functions can be accessed via the main dashboard:
- Manage accounting records (income/expense).
- Generate invoices and monitor payments.
- Track inventory and customer information.
- View financial reports.

### Example Usage
- **Track an Income**: Navigate to the "Income" section, enter the amount, and save the entry. The system will automatically update the cash flow.
- **Generate an Invoice**: Go to the "Invoices" section, select the customer and items, and generate the invoice.

## Future Improvements
- Integration with external payment gateways for payment monitoring.
- Enhanced reporting capabilities, such as real-time graphs and charts for better data visualization.
- Additional modules for more advanced accounting functions, such as tax calculation and payroll management.

## License
This project is licensed under the MIT License - see the LICENSE file for details.
