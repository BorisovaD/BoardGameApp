# 🎲 BoardGameApp

**BoardGameApp** is a web application for managing real-life board game clubs across different cities in Bulgaria.  
The platform allows users to explore clubs, book seats for specific game sessions or tournaments, add favorite games, and track their performance.

---

## ✨ Main Features

- 📍 Browse physical board game clubs by city and district
- 🗓 Book individual spots for board game sessions
- 🤝 Meet new people and form teams on the spot
- ⭐ Add favorite games to your personal list
- 📊 View player rankings and personal stats (wins/losses)
- 🧑‍💻 Admin area for managing games, sessions, and clubs

---

## ⚙️ Technologies Used

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- SQL Server
- Razor Views & Partial Views
- ASP.NET Identity (with User/Admin roles)
- Bootstrap for responsive UI
- GitHub for source control

---

## 🚀 How to Run the Project Locally

1. Clone the repository:
   ```bash
   git clone https://github.com/BorisovaD/BoardGameApp.git
  
2. Open the solution in Visual Studio 2022 (or later) or use dotnet CLI.

3. Make sure you have installed:

  - .NET 8 SDK

  - SQL Server (local or remote)

4. Update the connection string in appsettings.json if needed.

5. Apply the migrations to create the database:
   
dotnet ef database update

6. Run the project:
   
  dotnet run

7. Register a new user account.
To test Admin/Manager functionality, you can manually assign roles in the database.

8. Enjoy managing your board game club! 🎲
   
---

## 📄 License

This project is licensed under the MIT License.
See the LICENSE file for more details.

---

## 💬 Contact

For any questions or feedback, feel free to contact me:

- Email: dborisova@yahoo.com
- GitHub: [BorisovaD](https://github.com/BorisovaD)
