# 📚 LIBRIS 

![LIBRIS System Overview]
<img width="3033" height="1917" alt="1" src="https://github.com/user-attachments/assets/bf648b9f-ceef-4f1a-8eca-fcf933f9a1ca" />


> *A decoupled library management system featuring a .NET 8 Web API consumed by an MVC frontend. Built with Code-First Entity Framework, it leverages custom DTOs and Newtonsoft JSON for secure data transfer and robust admin CRUD operations.*

## 🚀 The Development Journey

As the 7th milestone in my Softito learning archive, **LIBRIS** represents a massive leap in architectural design. Moving beyond monolithic structures, this project implements a clean separation of concerns by completely decoupling the user interface from the data access layer. 

Building a standalone RESTful Web API and consuming it via an MVC client opened up an entirely new dimension of software development. By strictly utilizing **Data Transfer Objects (DTOs)**, the application ensures that internal database models are never directly exposed to the presentation layer, resulting in highly secure, scalable, and professional-grade code.

## 🛠️ Tech Stack & Architecture

### Backend (Web API)
* **Framework:** .NET 8 Web API
* **Database:** SQL Server via Entity Framework Core (Code-First)
* **Documentation:** Swagger UI
* **Architecture:** Repository Pattern concepts, Custom DTOs

### Frontend (MVC Client)
* **Framework:** ASP.NET Core MVC
* **API Consumption:** `IHttpClientFactory` & Newtonsoft.Json
* **UI/UX:** Custom "Cyber-Reading Room" Theme, HTML5/CSS3, Bootstrap 5
* **Libraries:** Chart.js (Data Visualization), Simple-Datatables, html2pdf.js (Snapshot Reporting)

---

## 📸 Feature Highlights

### Dynamic Dashboard & Instant Reporting
Features real-time KPI tracking and interactive data visualization using Chart.js. With a single click, the glowing neon "Generate Report" button instantly compiles the active DOM into a clean PDF snapshot.

![Add New Member]
<img width="3069" height="1917" alt="2" src="https://github.com/user-attachments/assets/d0fac54b-02a0-4998-a9f7-365185199197" />


### Seamless Circulation Tracking
Effortlessly manage library circulation. The system tracks active checkouts, monitors overdue returns, and links members to books via robust relational logic.

![Borrow Records Directory]
<img width="3024" height="1917" alt="3" src="https://github.com/user-attachments/assets/4a85b400-c319-4511-a63e-f7a6c642fe4b" />


### Secure & Validated CRUD Operations
Every entity (Books, Authors, Genres, Members) includes complete Create, Read, Update, and Delete capabilities. The MVC forms use ASP.NET Core Tag Helpers to enforce strict input validation before payload serialization.

![Edit Author Form]
<img width="3069" height="1917" alt="4" src="https://github.com/user-attachments/assets/b030c09f-11d0-4383-a790-5340ef2e52d1" />


### Lightning-Fast Datatables
All directory views are equipped with highly responsive, client-side data tables. Users can instantly search, sort by column, and paginate through hundreds of records without unnecessary round-trips to the server.

![Books Directory Search]
<img width="3069" height="1917" alt="5" src="https://github.com/user-attachments/assets/b7ba307d-a16e-497d-9269-580d7b667fd5" />


---
*Developed as Project #7 for the SoftITo Learning Archive.*
