# Smart Student Manager

## About the Project

Smart Student Manager is a C# Windows Forms application designed to manage student information, attendance, marks, and academic performance.

## Features

- Add and edit student details.
- Store student name, roll number, course, and gender.
- Enter marks for three subjects.
- Calculate average marks and grades automatically.
- Manage student attendance.
- Calculate attendance percentage.
- Generate student performance reports.
- View and print student report cards.
- Validate student details and prevent duplicate roll numbers.

## How to Run

1. Install **Visual Studio** on your computer.
2. Open Visual Studio and create a new **Windows Forms App** project using C#.
3. Set the project name to `SmartStudentManager`.
4. Add the provided C# source code files to the project.
5. Make sure the project has the required Windows Forms references.
6. Build the project by selecting **Build → Build Solution**.
7. Press **F5** or click **Start** to run the application.
8. The Smart Student Manager window will open, where you can add students, manage attendance, and generate performance reports.

**Note:** This is a Windows Forms desktop application. It cannot run directly on online C# console compilers such as OneCompiler.

## Technologies Used

- C#
- .NET Framework
- Windows Forms
- System.Drawing
- System.Windows.Forms
- LINQ

## OOP Concepts Used

- **Encapsulation:** Private data members are accessed through public properties.
- **Classes and Objects:** Different classes are used to manage students, attendance, and reports.
- **Inheritance:** Forms inherit from the Windows Forms `Form` class.
- **Abstraction:** Different operations are separated into individual classes and methods.

## Main Modules

- **Student Module:** Adds, edits, and manages student information.
- **Attendance Module:** Updates attendance records and calculates attendance percentage.
- **Performance Module:** Calculates average marks, grades, and results.
- **Report Module:** Displays and prints student performance reports.

## Data Storage

Student records are stored in an in-memory list during application execution. The data is not permanently saved, so it is lost when the application is closed.

## Conclusion

Smart Student Manager provides a simple way to manage student details, attendance, marks, and academic performance using C# and Windows Forms.
