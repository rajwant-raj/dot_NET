using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace SmartStudentManager
{
    public class Student
    {
        public string Name { get; set; }
        public string RollNo { get; set; }
        public string Course { get; set; }
        public string Gender { get; set; }
        public decimal Mark1 { get; set; }
        public decimal Mark2 { get; set; }
        public decimal Mark3 { get; set; }
        public int TotalClasses { get; set; }
        public int AttendedClasses { get; set; }

        public decimal Average
        {
            get { return (Mark1 + Mark2 + Mark3) / 3m; }
        }

        public decimal AttendancePercentage
        {
            get
            {
                if (TotalClasses == 0) return 0;
                return (decimal)AttendedClasses / TotalClasses * 100m;
            }
        }

        public string Grade
        {
            get
            {
                if (Average >= 90) return "A+";
                if (Average >= 80) return "A";
                if (Average >= 70) return "B";
                if (Average >= 60) return "C";
                if (Average >= 50) return "D";
                return "F";
            }
        }

        public string Result
        {
            get { return Average >= 40 ? "PASS" : "FAIL"; }
        }

        public override string ToString()
        {
            return RollNo + " - " + Name;
        }
    }

    public static class StudentData
    {
        public static List<Student> Students = new List<Student>();
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private DataGridView grid;

        public MainForm()
        {
            Text = "Smart Student Record and Performance Manager";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1050, 650);
            MinimumSize = new Size(850, 550);
            BackColor = Color.WhiteSmoke;

            Label heading = new Label
            {
                Text = "SMART STUDENT RECORD AND PERFORMANCE MANAGER",
                Dock = DockStyle.Top,
                Height = 65,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                BackColor = Color.White
            };
            Controls.Add(heading);

            FlowLayoutPanel buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 65,
                Padding = new Padding(12),
                BackColor = Color.Gainsboro
            };

            Button addButton = MakeButton("Add Student");
            Button editButton = MakeButton("Edit Selected");
            Button attendanceButton = MakeButton("Manage Attendance");
            Button reportButton = MakeButton("Performance / Report Card");
            Button refreshButton = MakeButton("Refresh");

            addButton.Click += (s, e) =>
            {
                using (StudentForm form = new StudentForm(null))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        RefreshGrid();
                }
            };

            editButton.Click += (s, e) =>
            {
                Student selected = GetSelectedStudent();

                if (selected == null)
                {
                    MessageBox.Show("Please select a student first.");
                    return;
                }

                using (StudentForm form = new StudentForm(selected))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        RefreshGrid();
                }
            };

            attendanceButton.Click += (s, e) =>
            {
                using (AttendanceForm form = new AttendanceForm())
                {
                    form.ShowDialog();
                    RefreshGrid();
                }
            };

            reportButton.Click += (s, e) =>
            {
                Student selected = GetSelectedStudent();

                if (selected == null)
                {
                    MessageBox.Show("Please select a student first.");
                    return;
                }

                using (ReportForm form = new ReportForm(selected))
                    form.ShowDialog();
            };

            refreshButton.Click += (s, e) => RefreshGrid();

            buttons.Controls.Add(addButton);
            buttons.Controls.Add(editButton);
            buttons.Controls.Add(attendanceButton);
            buttons.Controls.Add(reportButton);
            buttons.Controls.Add(refreshButton);

            Controls.Add(buttons);

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            Controls.Add(grid);

            grid.BringToFront();
            buttons.BringToFront();
            heading.BringToFront();

            RefreshGrid();
        }

        private Button MakeButton(string text)
        {
            return new Button
            {
                Text = text,
                Width = 175,
                Height = 38,
                Margin = new Padding(5),
                BackColor = Color.FromArgb(35, 85, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
        }

        private Student GetSelectedStudent()
        {
            if (grid.CurrentRow == null ||
                grid.CurrentRow.Cells["RollNumber"].Value == null)
                return null;

            string roll = Convert.ToString(
                grid.CurrentRow.Cells["RollNumber"].Value);

            return StudentData.Students.FirstOrDefault(
                x => x.RollNo == roll);
        }

        private void RefreshGrid()
        {
            grid.DataSource = null;

            grid.DataSource = StudentData.Students.Select(s => new
            {
                Name = s.Name,
                RollNumber = s.RollNo,
                Course = s.Course,
                Gender = s.Gender,
                AverageMarks = s.Average.ToString("0.00"),
                Attendance = s.AttendancePercentage.ToString("0.00") + "%",
                Grade = s.Grade,
                Result = s.Result
            }).ToList();

            if (grid.Columns.Count > 0)
            {
                grid.Columns["RollNumber"].HeaderText = "Roll Number";
                grid.Columns["AverageMarks"].HeaderText = "Average Marks";
            }
        }
    }

    public class StudentForm : Form
    {
        private TextBox nameBox, rollBox, courseBox;
        private RadioButton maleRadio, femaleRadio, otherRadio;
        private NumericUpDown mark1, mark2, mark3;
        private Student editingStudent;

        public StudentForm(Student student)
        {
            editingStudent = student;

            Text = student == null ? "Add Student" : "Edit Student";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(470, 510);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.WhiteSmoke;

            TableLayoutPanel panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                ColumnCount = 2,
                RowCount = 9
            };

            panel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 35));

            panel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 65));

            Controls.Add(panel);

            AddLabel(panel, "Student Name:", 0);
            nameBox = AddTextBox(panel, 0);

            AddLabel(panel, "Roll Number:", 1);
            rollBox = AddTextBox(panel, 1);

            AddLabel(panel, "Course / Branch:", 2);
            courseBox = AddTextBox(panel, 2);

            AddLabel(panel, "Gender:", 3);

            FlowLayoutPanel genderPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill
            };

            maleRadio = new RadioButton
            {
                Text = "Male",
                AutoSize = true
            };

            femaleRadio = new RadioButton
            {
                Text = "Female",
                AutoSize = true
            };

            otherRadio = new RadioButton
            {
                Text = "Other",
                AutoSize = true
            };

            genderPanel.Controls.Add(maleRadio);
            genderPanel.Controls.Add(femaleRadio);
            genderPanel.Controls.Add(otherRadio);

            panel.Controls.Add(genderPanel, 1, 3);

            AddLabel(panel, "Subject 1 Marks:", 4);
            mark1 = AddMarkControl(panel, 4);

            AddLabel(panel, "Subject 2 Marks:", 5);
            mark2 = AddMarkControl(panel, 5);

            AddLabel(panel, "Subject 3 Marks:", 6);
            mark3 = AddMarkControl(panel, 6);

            Button saveButton = new Button
            {
                Text = "Save Student",
                Dock = DockStyle.Fill,
                Height = 40,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            saveButton.Click += SaveStudent;
            panel.Controls.Add(saveButton, 1, 7);

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Dock = DockStyle.Fill,
                Height = 35
            };

            cancelButton.Click += (s, e) => Close();
            panel.Controls.Add(cancelButton, 1, 8);

            if (student != null)
            {
                nameBox.Text = student.Name;
                rollBox.Text = student.RollNo;
                rollBox.Enabled = false;
                courseBox.Text = student.Course;

                maleRadio.Checked = student.Gender == "Male";
                femaleRadio.Checked = student.Gender == "Female";
                otherRadio.Checked = student.Gender == "Other";

                mark1.Value = student.Mark1;
                mark2.Value = student.Mark2;
                mark3.Value = student.Mark3;
            }
        }

        private void AddLabel(
            TableLayoutPanel panel, string text, int row)
        {
            panel.Controls.Add(new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            }, 0, row);
        }

        private TextBox AddTextBox(
            TableLayoutPanel panel, int row)
        {
            TextBox box = new TextBox
            {
                Dock = DockStyle.Fill
            };

            panel.Controls.Add(box, 1, row);
            return box;
        }

        private NumericUpDown AddMarkControl(
            TableLayoutPanel panel, int row)
        {
            NumericUpDown control = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 2,
                Dock = DockStyle.Left,
                Width = 180
            };

            panel.Controls.Add(control, 1, row);
            return control;
        }

        private void SaveStudent(object sender, EventArgs e)
        {
            string name = nameBox.Text.Trim();
            string roll = rollBox.Text.Trim();
            string course = courseBox.Text.Trim();

            if (name == "" || roll == "" || course == "")
            {
                MessageBox.Show(
                    "Please fill in the name, roll number and course.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!maleRadio.Checked &&
                !femaleRadio.Checked &&
                !otherRadio.Checked)
            {
                MessageBox.Show("Please select gender.");
                return;
            }

            if (editingStudent == null &&
                StudentData.Students.Any(
                    s => s.RollNo.Equals(
                        roll,
                        StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("This roll number already exists.");
                return;
            }

            Student target = editingStudent ?? new Student();

            target.Name = name;
            target.RollNo = roll;
            target.Course = course;

            target.Gender = maleRadio.Checked ? "Male" :
                            femaleRadio.Checked ? "Female" : "Other";

            target.Mark1 = mark1.Value;
            target.Mark2 = mark2.Value;
            target.Mark3 = mark3.Value;

            if (editingStudent == null)
                StudentData.Students.Add(target);

            MessageBox.Show("Student record saved successfully.");

            DialogResult = DialogResult.OK;
            Close();
        }
    }

    public class AttendanceForm : Form
    {
        private ComboBox studentCombo;
        private NumericUpDown totalBox, attendedBox;

        public AttendanceForm()
        {
            Text = "Attendance Management";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(430, 310);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            TableLayoutPanel panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                ColumnCount = 2,
                RowCount = 5
            };

            panel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 40));

            panel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 60));

            Controls.Add(panel);

            panel.Controls.Add(new Label
            {
                Text = "Select Student:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 0);

            studentCombo = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = StudentData.Students.ToList()
            };

            panel.Controls.Add(studentCombo, 1, 0);

            panel.Controls.Add(new Label
            {
                Text = "Total Classes:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 1);

            totalBox = MakeNumber();
            panel.Controls.Add(totalBox, 1, 1);

            panel.Controls.Add(new Label
            {
                Text = "Attended Classes:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 2);

            attendedBox = MakeNumber();
            panel.Controls.Add(attendedBox, 1, 2);

            Button save = new Button
            {
                Text = "Save Attendance",
                Dock = DockStyle.Fill,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White
            };

            save.Click += SaveAttendance;
            panel.Controls.Add(save, 1, 3);

            Button close = new Button
            {
                Text = "Close",
                Dock = DockStyle.Fill
            };

            close.Click += (s, e) => Close();
            panel.Controls.Add(close, 1, 4);

            studentCombo.SelectedIndexChanged += LoadAttendance;

            if (studentCombo.Items.Count > 0)
                LoadAttendance(null, EventArgs.Empty);
        }

        private NumericUpDown MakeNumber()
        {
            return new NumericUpDown
            {
                Minimum = 0,
                Maximum = 10000,
                Dock = DockStyle.Left,
                Width = 180
            };
        }

        private void LoadAttendance(object sender, EventArgs e)
        {
            Student student = studentCombo.SelectedItem as Student;

            if (student != null)
            {
                totalBox.Value = student.TotalClasses;
                attendedBox.Value = student.AttendedClasses;
            }
        }

        private void SaveAttendance(object sender, EventArgs e)
        {
            Student student = studentCombo.SelectedItem as Student;

            if (student == null)
            {
                MessageBox.Show(
                    "Add a student before recording attendance.");
                return;
            }

            if (attendedBox.Value > totalBox.Value)
            {
                MessageBox.Show(
                    "Attended classes cannot exceed total classes.");
                return;
            }

            student.TotalClasses = (int)totalBox.Value;
            student.AttendedClasses = (int)attendedBox.Value;

            MessageBox.Show(
                "Attendance saved. Attendance: " +
                student.AttendancePercentage.ToString("0.00") + "%");
        }
    }

    public class ReportForm : Form
    {
        private Student student;
        private PrintDocument printDocument;

        public ReportForm(Student selectedStudent)
        {
            student = selectedStudent;

            Text = "Student Performance Report";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(550, 530);
            BackColor = Color.White;

            Label heading = new Label
            {
                Text = "STUDENT PERFORMANCE REPORT CARD",
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            Controls.Add(heading);

            TextBox details = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 11),
                Dock = DockStyle.Fill,
                Text = BuildReport()
            };

            Controls.Add(details);
            details.BringToFront();
            heading.BringToFront();

            FlowLayoutPanel bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(8)
            };

            Button printButton = new Button
            {
                Text = "Print Preview",
                Width = 130,
                Height = 35
            };

            Button closeButton = new Button
            {
                Text = "Close",
                Width = 90,
                Height = 35
            };

            printButton.Click += ShowPrintPreview;
            closeButton.Click += (s, e) => Close();

            bottom.Controls.Add(printButton);
            bottom.Controls.Add(closeButton);

            Controls.Add(bottom);
            bottom.BringToFront();

            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintReport;
        }

        private string BuildReport()
        {
            return
                "==================================================\r\n" +
                "                 STUDENT REPORT CARD              \r\n" +
                "==================================================\r\n\r\n" +

                "Student Name       : " + student.Name + "\r\n" +
                "Roll Number        : " + student.RollNo + "\r\n" +
                "Course / Branch    : " + student.Course + "\r\n" +
                "Gender             : " + student.Gender + "\r\n\r\n" +

                "---------------- ACADEMIC DETAILS ----------------\r\n" +

                "Subject 1 Marks    : " + student.Mark1 + " / 100\r\n" +
                "Subject 2 Marks    : " + student.Mark2 + " / 100\r\n" +
                "Subject 3 Marks    : " + student.Mark3 + " / 100\r\n" +

                "Average Marks      : " +
                student.Average.ToString("0.00") + "%\r\n" +

                "Grade              : " + student.Grade + "\r\n" +
                "Result             : " + student.Result + "\r\n\r\n" +

                "---------------- ATTENDANCE DETAILS --------------\r\n" +

                "Total Classes      : " + student.TotalClasses + "\r\n" +
                "Attended Classes   : " + student.AttendedClasses + "\r\n" +

                "Attendance         : " +
                student.AttendancePercentage.ToString("0.00") + "%\r\n\r\n" +

                "==================================================\r\n" +
                "Generated by Smart Student Record Manager\r\n";
        }

        private void ShowPrintPreview(object sender, EventArgs e)
        {
            using (PrintPreviewDialog preview = new PrintPreviewDialog())
            {
                preview.Document = printDocument;
                preview.Width = 900;
                preview.Height = 700;
                preview.ShowDialog(this);
            }
        }

        private void PrintReport(object sender, PrintPageEventArgs e)
        {
            using (Font font = new Font("Consolas", 11))
            using (Brush brush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString(
                    BuildReport(),
                    font,
                    brush,
                    new RectangleF(
                        50,
                        50,
                        e.MarginBounds.Width,
                        e.MarginBounds.Height));
            }
        }
    }
}

