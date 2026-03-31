# 🧺 Survey Basket System

A full-stack **Survey & Poll Management System** built with **ASP.NET Core (Backend)** and **Angular (Frontend)**, designed to handle dynamic surveys, voting, analytics, and user management with a clean and scalable architecture.

---

## 🚀 Overview

Survey Basket enables organizations to:

* Create and manage surveys (polls)
* Add dynamic questions with multiple answers
* Collect and analyze votes
* Manage users and roles
* Visualize insights through dashboards

The system follows modern best practices in both backend and frontend development, ensuring scalability, maintainability, and performance.

---

## 🏗️ Architecture

### 🔙 Backend (ASP.NET Core)

* RESTful API with OpenAPI (Swagger)
* JWT Authentication + Refresh Tokens
* Role-Based Authorization
* Entity Framework Core
* API Versioning (`x-api-version`)
* Modular feature-based structure

### 🎨 Frontend (Angular 21)

* Standalone Components (No NgModules)
* Angular Material UI
* Feature-based architecture
* Reactive Forms
* HTTP Interceptors (Auth + Error handling)
* Signals + RxJS
* Lazy Loading

---

## 📦 Features

### 🔐 Authentication & Security

* User Registration & Login
* Email Confirmation
* Forgot / Reset Password
* JWT + Refresh Token Flow
* Role-Based Access Control (RBAC)

### 📊 Poll Management

* Create, Update, Delete Polls
* Publish / Unpublish Polls
* Schedule Polls (Start / End Date)

### ❓ Questions Management

* Add / Edit / Delete Questions
* Multiple Answers Support
* Pagination, Searching, Sorting

### 🗳️ Voting System

* Submit Votes per Poll
* Prevent invalid submissions
* Structured vote payload handling

### 📈 Dashboard & Analytics

* Total Votes per Poll
* Votes per Day
* Votes per Answer (distribution)

### 👥 User & Role Management

* Manage Users
* Assign Roles
* Update User Information

---

## 🛠️ Tech Stack

### Backend

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* JWT Authentication
* Swagger / OpenAPI

### Frontend

* Angular 21.2.4
* Angular Material
* RxJS + Signals
* SCSS

---

## ⚙️ Getting Started

### 🔧 Prerequisites

* .NET 10 SDK
* Node.js (>= 18)
* Angular CLI
* SQL Server

---

### ▶️ Backend Setup

```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

API will run at:

```
https://localhost:5001
```

Swagger:

```
https://localhost:5001/swagger
```

---

### ▶️ Frontend Setup

```bash
cd frontend
npm install
ng serve
```

App will run at:

```
http://localhost:61933
```

---

## 🔐 Authentication Flow

1. User logs in → receives:

   * Access Token
   * Refresh Token
2. Access token is sent in:

```
Authorization: Bearer {token}
```

3. Refresh token used automatically when expired

---

## 📡 API Notes

* All endpoints require header:

```
x-api-version: 1.0
```

* Protected endpoints require JWT Bearer Token

---

## 📊 Example Modules

| Module    | Description                     |
| --------- | ------------------------------- |
| Auth      | Login, Register, Token handling |
| Polls     | Manage surveys                  |
| Questions | Manage poll questions           |
| Votes     | Submit and process votes        |
| Dashboard | Analytics and reporting         |
| Users     | Admin user management           |

---


## 🔒 Security Considerations

* JWT secured endpoints
* Role-based authorization
* Input validation
* Secure password handling
* Token refresh strategy

---

## 👨‍💻 Author

Ahmed Mahmoud
📧 [ahmed.mahmoud.6618@gmail.com](mailto:ahmed.mahmoud.6618@gmail.com)

---

## 📄 License

This project is licensed under the MIT License.

---

## ⭐ Support

If you like this project, give it a ⭐ on GitHub!
