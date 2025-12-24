# CoreGym Booking System

![.NET Core](https://img.shields.io/badge/.NET%20Core-512BD4?style=flat&logo=dotnet&logoColor=white)
![Razor Pages](https://img.shields.io/badge/Razor%20Pages-512BD4?style=flat&logo=dotnet&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-512BD4?style=flat&logo=nuget&logoColor=white)
![Agile](https://img.shields.io/badge/Methodology-Agile-blue)

**CoreGym Booking System** is a comprehensive gym management application built with **ASP.NET Core Razor Pages**. Developed by **Group A** using **Agile working methods**, this project demonstrates effective team collaboration and iterative software design to streamline operations for gym owners, trainers, and members.

## 🚀 Features

### For Members
* **User Dashboard:** View active memberships, upcoming classes, and booking history.
* **Class Booking:** Browse the schedule and book available sessions instantly.
* **Membership Management:** Sign up for new membership plans.

### For Trainers
* **Trainer Dashboard:** Manage assigned classes and view attendee lists.
* **Session Management:** Create and update workout sessions.

### For Administrators
* **Full Control:** Complete CRUD access to Users, Trainers, and Classes.
* **Insights:** Dashboard for monitoring gym activity and user statistics.
* **Notification System:** Manage announcements and user alerts.

## 🔄 Development Methodology

This project was built by a collaborative team adhering to **Agile principles**. Key aspects of the development process included:

* **Iterative Development:** Features were implemented in sprints, allowing for continuous feedback and improvement.
* **Team Collaboration:** Utilizing version control and code reviews to ensure code quality across the team.
* **Task Management:** Workflow managed through GitHub Projects using **User Stories** and **Tasks** to track progress and requirements.

## 🏗 Architecture

The solution follows a clean **N-Tier Architecture** to ensure separation of concerns and maintainability:

* **MainApp (Presentation Layer):** ASP.NET Core Razor Pages handling the UI and user interaction.
* **Service (Business Logic Layer):** Contains all business logic, services (BookingService, UserService, etc.), and interfaces.
* **DAL (Data Access Layer):** Handles database connections using Entity Framework Core, Repositories, and Entities.
* **Tests:** Separate projects for Unit and Integration testing (`DAL.Tests`, `Service.Tests`).

## 🛠 Tech Stack

* **Framework:** ASP.NET Core 8.0 (Razor Pages)
* **Database:** SQL Server
* **ORM:** Entity Framework Core
* **Identity:** ASP.NET Core Identity for authentication and authorization
* **Frontend:** Bootstrap 5, HTML5, CSS3
* **Testing:** xUnit

## ⚙️ Getting Started

Follow these steps to get the project running locally.

### Prerequisites
* [.NET SDK](https://dotnet.microsoft.com/download) (Version 8.0 or later recommended)
* SQL Server (LocalDB or full instance)
* Visual Studio 2022 or VS Code

### Installation

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/your-username/CoreGymBookingSystem.git](https://github.com/your-username/CoreGymBookingSystem.git)
    cd CoreGymBookingSystem
    ```

2.  **Configure the Database:**
    Update the connection string in `MainApp/appsettings.json` to point to your local SQL Server instance.

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CoreGymDB;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

3.  **Apply Migrations:**
    Navigate to the solution folder and run the database update command to create the schema and seed initial data.

    ```bash
    dotnet ef database update --project CoreGymBookingSystem/DAL --startup-project CoreGymBookingSystem/MainApp
    ```

4.  **Run the Application:**
    ```bash
    dotnet run --project CoreGymBookingSystem/MainApp
    ```

5.  **Access the App:**
    Open your browser and navigate to `https://localhost:7146` (or the port indicated in your terminal).

## 🧪 Running Tests

To run the unit and integration tests, use the following command in the solution root:

```bash
dotnet test
````

### 👥 Authors
Group A - Development Team - [Kyh Agile Project]

### 📄 License
This project is licensed under the MIT License.
