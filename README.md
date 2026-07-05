# 📚 LIBRIS 
<img width="3033" height="1917" alt="1" src="https://github.com/user-attachments/assets/bf648b9f-ceef-4f1a-8eca-fcf933f9a1ca" />


> *A decoupled library management system featuring a .NET 8 Web API consumed by an MVC frontend. Built with Code-First Entity Framework, it leverages custom DTOs and Newtonsoft JSON for secure data transfer and robust admin CRUD operations. The system is secured end-to-end with ASP.NET Core Identity — the API owns the credential store and issues stateless auth verdicts, while the MVC client manages the actual browser session via cookie authentication.*

## 🚀 The Development Journey

As the 7th milestone in my Softito learning archive, **LIBRIS** represents a massive leap in architectural design. Moving beyond monolithic structures, this project implements a clean separation of concerns by completely decoupling the user interface from the data access layer. 

Building a standalone RESTful Web API and consuming it via an MVC client opened up an entirely new dimension of software development. By strictly utilizing **Data Transfer Objects (DTOs)**, the application ensures that internal database models are never directly exposed to the presentation layer, resulting in highly secure, scalable, and professional-grade code.

Beyond the CRUD and reporting layers, LIBRIS was also where I tackled a genuinely new architectural question: in a decoupled API/MVC system, which layer should own authentication? The answer — the layer that owns the user data — meant giving the API project full ASP.NET Core Identity integration (UserManager, SignInManager, hashed credentials), while the MVC client calls those endpoints over HTTP and only concerns itself with issuing and validating its own browser cookie. It's a small distinction on paper, but it's the difference between bolting auth onto whichever project is easiest and actually respecting the separation of concerns the rest of this project was built around.

## 🛠️ Tech Stack & Architecture

### Backend (Web API)
* **Framework:** .NET 8 Web API
* **Database:** SQL Server via Entity Framework Core (Code-First)
* **Authentication:** ASP.NET Core Identity (EF Core store) — credential hashing, lockout policy, token-based password reset; exposed as stateless REST endpoints (`/api/auth/*`) rather than cookie-issuing actions
* **Documentation:** Swagger UI
* **Architecture:** Repository Pattern concepts, Custom DTOs

### Frontend (MVC Client)
* **Framework:** ASP.NET Core MVC
* **API Consumption:** `IHttpClientFactory` & Newtonsoft.Json
* **Session Management:** ASP.NET Core Cookie Authentication — no direct database or EF Core access; login/session state is established locally after a successful API auth response
* **UI/UX:** Custom "Cyber-Reading Room" Theme, HTML5/CSS3, Bootstrap 5
* **Libraries:** Chart.js (Data Visualization), Simple-Datatables, html2pdf.js (Snapshot Reporting)

---

## 📸 Feature Highlights

### Dynamic Dashboard & Instant Reporting
Features real-time KPI tracking and interactive data visualization using Chart.js. [cite_start]With a single click, the glowing neon "Generate Report" button instantly compiles the active dashboard view into a clean, professional PDF snapshot.
<img width="3069" height="1917" alt="6" src="https://github.com/user-attachments/assets/66e0bfcd-e6ed-437c-a66a-24582032c870" />
<img width="3069" height="1917" alt="2" src="https://github.com/user-attachments/assets/d0fac54b-02a0-4998-a9f7-365185199197" />


### Seamless Circulation Tracking
Effortlessly manage library circulation. The system tracks active checkouts, monitors overdue returns, and links members to books via robust relational logic.
<img width="3024" height="1917" alt="3" src="https://github.com/user-attachments/assets/4a85b400-c319-4511-a63e-f7a6c642fe4b" />


### Secure & Validated CRUD Operations
Every entity (Books, Authors, Genres, Members) includes complete Create, Read, Update, and Delete capabilities. The MVC forms use ASP.NET Core Tag Helpers to enforce strict input validation before payload serialization.
<img width="3069" height="1917" alt="4" src="https://github.com/user-attachments/assets/b030c09f-11d0-4383-a790-5340ef2e52d1" />


### Lightning-Fast Datatables
All directory views are equipped with highly responsive, client-side data tables. Users can instantly search, sort by column, and paginate through hundreds of records without unnecessary round-trips to the server.
<img width="3069" height="1917" alt="5" src="https://github.com/user-attachments/assets/b7ba307d-a16e-497d-9269-580d7b667fd5" />

### Later Added: Login/Signup/Edit Account Details Functionalities
Admin access is fully gated behind ASP.NET Core Identity, with the credential store living in the API layer and the MVC client handling only its own signed-in session. The flow covers registration, login, an authenticated password-change screen requiring the current password, a token-based forgot-password recovery path (the reset token is surfaced on-screen in place of a real email service), and a profile page for updating name and email — all routed through the API's `/api/auth/*` endpoints rather than direct database access from the MVC side.
<img width="3069" height="1917" alt="x" src="https://github.com/user-attachments/assets/9d107ef2-140f-475c-9822-a9adcc2b03fa" />
<img width="3069" height="1917" alt="y" src="https://github.com/user-attachments/assets/d8355540-5cd4-4b54-beea-0c6372eb542e" />
<img width="3069" height="1917" alt="z" src="https://github.com/user-attachments/assets/dc86e9d3-2f45-4ed8-b0b8-d3f8b49be4dc" />
<img width="3069" height="1917" alt="t" src="https://github.com/user-attachments/assets/216bd6b2-b71a-4a0d-a9e6-d87cc15fff27" />

---

