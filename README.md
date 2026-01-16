🏋️ **GymFlow**



GymFlow is a **full-stack gym management and booking platform** built with **Angular** (frontend) and **ASP.NET Core Web API** (backend).

It supports role-based access for **Admins**, **Instructors**, and **Clients**, enabling trainer discovery, availability scheduling, slot-based bookings, and direct messaging.



This project was built to demonstrate **real-world full-stack development**, secure API design, and clean application architecture.



📸 Screenshots



API Documentation (Swagger)







\### Trainer Search \& Browse



!\[Trainer Search](./screenshots/trainer-search.png)



\### Booking \& Availability



!\[Booking Slots](./screenshots/booking-slots.png)



\### Instructor Dashboard



!\[Instructor Dashboard](./screenshots/instructor-dashboard.png)



\### Chat System



!\[Chat UI](./screenshots/chat-ui.png)



---



\## 🚀 Features Overview



\### 🔐 Authentication \& Authorization



\* JWT authentication (access \& refresh tokens)

\* Role-based authorization (Admin / Instructor / Client)

\* Angular route guards for protected pages

\* Secure claims-based user identity



---



\### 🧑‍🏫 Instructor Features



\* Instructor profile linked to user account

\* Edit profile (bio, specialty)

\* Upload profile image (fallback avatar using first letter)

\* Create weekly availability using \*\*DayOfWeek + TimeSpan\*\*

\* Backend overlap-prevention for availability

\* Automatic generation of \*\*30-minute booking slots\*\*

\* View assigned clients

\* View all bookings

\* Cancel bookings



---



\### 🧍 Client Features



\* Browse all trainers

\* Search trainers by name or specialty

\* View instructor profiles

\* View weekly availability

\* Book available time slots

\* Already-booked slots disabled

\* Booking confirmation modal

\* Dark-mode friendly UI



---



\### 🛠 Admin Features



\* Promote users to Instructor role

\* Secure admin-only endpoints



---



\### 💬 Chat System



\* One-to-one messaging (Client ↔ Instructor)

\* Sender/receiver-based conversations

\* Unread message count per user

\* Notification badge for unread messages

\* Supports text and image messages

\* REST-based (ready for SignalR real-time upgrade)



---



\## 🧱 Tech Stack



\### Frontend



\* Angular (Standalone Components)

\* TypeScript

\* Tailwind CSS + DaisyUI

\* Angular Route Guards

\* REST API services

\* Toast notifications \& modals



\### Backend



\* ASP.NET Core Web API

\* Entity Framework Core

\* SQL Server

\* JWT Authentication (Access \& Refresh Tokens)

\* Role-based Authorization

\* Swagger API documentation

\* Static file hosting (`wwwroot/uploads`)



---



\## 📂 Project Structure



```

GymFlow/

├── gym-app/                # Angular frontend

├── GymflowBackend/         # ASP.NET Core backend

├── screenshots/            # README images

└── README.md

```



---



\## 📑 API Overview



The backend exposes a \*\*RESTful API\*\* documented using \*\*Swagger\*\*.



Main API areas include:



\* Authentication \& authorization

\* User and role management

\* Trainer discovery \& search

\* Instructor availability management

\* Slot-based booking system

\* Booking cancellation

\* Chat messaging and unread counts



Swagger UI is available when running the backend.



---



\## 🧠 Booking \& Availability Logic



\* Instructors define weekly availability by day and time range

\* Backend prevents overlapping availability entries

\* Availability is converted into \*\*30-minute booking slots\*\*

\* Booked slots are returned with an `isBooked` flag

\* Backend enforces \*\*no double booking\*\*

\* Bookings store precise `startDateTime` and `endDateTime`



---



\## ▶️ Running the Project Locally



\### Backend (ASP.NET Core)



1\. Open the backend project

2\. Configure SQL Server connection string

3\. Run Entity Framework migrations

4\. Start the API

5\. Access Swagger at `/swagger`



\### Frontend (Angular)



1\. Navigate to the frontend folder

2\. Install dependencies

3\. Start the Angular development server

4\. Open the app in the browser



---



\## 📌 Resume-Ready Highlights



\* Built a full-stack gym scheduling and booking system

\* Implemented JWT authentication with refresh token support

\* Designed role-based dashboards for Admin, Instructor, and Client

\* Developed conflict-free slot-based booking logic

\* Created REST-based chat system with unread message tracking

\* Integrated image uploads with static file hosting

\* Used Angular standalone components and modern UI tooling



---



\## 🔮 Future Improvements



\* Real-time chat with SignalR

\* Calendar-based booking interface

\* Admin analytics dashboard

\* Email notifications

\* Payment integration



---



\## 👤 Author



\*\*Your Name\*\*

Computer Science Student | Full-Stack Developer

GitHub: \*your-github-link\*



---



\## ✅ What to Do Next (Checklist)



1\. Create a folder called \*\*`screenshots`\*\*

2\. Add at least:



&nbsp;  \* `swagger-api.png` ✅ (you already have this)

3\. Paste this README into `README.md`

4\. Push to GitHub



If you want, next I can:



\* 📸 Tell you \*\*exactly how to take each screenshot\*\*

\* ✂️ Shorten this for \*\*hackathon / portfolio version\*\*

\* 💼 Rewrite it to match \*\*specific internship job postings\*\*

\* 🧾 Split frontend/backend READMEs



Just tell me 👍



