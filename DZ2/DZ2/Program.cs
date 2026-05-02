using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string dbPath = "students.db";
string facCsv = @"D:\pr\csharp\DZ2\DZ2\bin\Debug\net10.0\fac.csv";
string stuCsv = @"D:\pr\csharp\DZ2\DZ2\bin\Debug\net10.0\stu.csv";

var db = new DatabaseManager(dbPath);
db.InitializeDatabase(facCsv, stuCsv);

// Главный цикл меню
string choice;
do
{
    Console.Clear();
    Console.WriteLine("╔══════════════════════════════════════╗");
    Console.WriteLine("║ УПРАВЛЕНИЕ СТУДЕНТАМИ                ║");
    Console.WriteLine("╠══════════════════════════════════════╣");
    Console.WriteLine("║ 1 — Показать все факультеты          ║");
    Console.WriteLine("║ 2 — Показать всех студентов          ║");
    Console.WriteLine("║ 3 — Добавить студента                ║");
    Console.WriteLine("║ 4 — Редактировать студента           ║");
    Console.WriteLine("║ 5 — Удалить студента                 ║");
    Console.WriteLine("║ 6 — Отчёты                           ║");
    Console.WriteLine("║ 0 — Выход                            ║");
    Console.WriteLine("╚══════════════════════════════════════╝");
    Console.Write("Ваш выбор: ");
    choice = Console.ReadLine()?.Trim() ?? "";
    Console.WriteLine();
    Console.Clear();
    switch (choice)
    {   
        case "1": Showfaculties(db); break;
        case "2": ShowStudents(db); break;
        case "3": AddStudent(db); break;
        case "4": EditStudent(db); break;
        case "5": DeleteStudent(db); break;
        case "6": ReportsMenu(db); break;
        case "0": Console.WriteLine("Конец программы"); break;
        default: Console.WriteLine("Неверный пункт меню."); break;
    }
    Console.Write("\nНажмите любую кнопку чтобы вернуться в меню ...");
    Console.ReadKey();
    Console.WriteLine();
}
while (choice != "0");

static void Showfaculties(DatabaseManager db)
{
    Console.WriteLine("--- Все факультеты ---");
    var faculties = db.GetAllFaculties();
    foreach (var fac in faculties)
        Console.WriteLine(" " + fac);
    Console.WriteLine($"Итого: {faculties.Count}");
}
static void ShowStudents(DatabaseManager db)
{
    Console.WriteLine("--- Все студенты ---");
    var students = db.GetAllStudents();
    foreach (var stu in students)
        Console.WriteLine(" " + stu);
    Console.WriteLine($"Итого: {students.Count}");
}

static void AddStudent(DatabaseManager db)
{
    Console.WriteLine("--- Добавление студента ---");
    Console.WriteLine("Доступные факультеты:");
    var faculties = db.GetAllFaculties();
    foreach (var fac in faculties)
        Console.WriteLine(" " + fac);

    Console.Write("ID факультета: ");
    if (!int.TryParse(Console.ReadLine(), out int facId))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }
    var faculty = db.GetAllFaculties().FirstOrDefault(f => f.Id == facId);
    if (faculty == null)
    {
        Console.WriteLine($"Ошибка: факультет с ID={facId} не найден.");
        return;
    }
    Console.Write("Имя студента: ");
    string name = Console.ReadLine()?.Trim() ?? "";
    if (name.Length == 0)
    {
        Console.WriteLine("Ошибка: имя не может быть пустым.");
        return;
    }
    Console.Write("Средний балл: ");
    if (!double.TryParse(Console.ReadLine(), out double gpa))
    {
        Console.WriteLine("Ошибка: введите число.");
        return;
    }
    try
    {
        var stu = new Student(0, facId, name, gpa);
        db.AddStudent(stu);
        Console.WriteLine("Студент добавлен.");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}
static void EditStudent(DatabaseManager db)
{
    Console.WriteLine("--- Редактирование студента ---");
    Console.Write("Введите ID студента: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }
    var stu = db.GetStudentById(id);
    if (stu == null)
    {
        Console.WriteLine($"Студент с ID={id} не найден.");
        return;
    }
    Console.WriteLine($"Текущие данные: {stu}");
    Console.WriteLine("(нажмите Enter, чтобы оставить значение без изменений)");

    Console.Write($"Имя [{stu.Name}]: ");
    string input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0)
        stu.Name = input;
    Console.Write($"ID факультета [{stu.FacultyId}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && int.TryParse(input, out int newFacId))
    {
        var faculties = db.GetAllFaculties();
        if (faculties.Any(f => f.Id == newFacId))
        {
            stu.FacultyId = newFacId;
        }
        else
        {
            Console.WriteLine($"Ошибка: факультет с ID={newFacId} не найден. Оставлено прежнее значение.");
        }
    }

    Console.Write($"Средние баллы [{stu.Gpa}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && double.TryParse(input, out double newGpa))
    {
        try
        {
            stu.Gpa = newGpa; 
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return;
        }
    }
    db.UpdateStudent(stu);
    Console.WriteLine("Данные обновлены.");
}

static void DeleteStudent(DatabaseManager db)
{
    Console.WriteLine("--- Удаление студента ---");
    Console.Write("Введите ID студента: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }
    var stu = db.GetStudentById(id);
    if (stu == null)
    {
        Console.WriteLine($"Студент с ID={id} не найден.");
        return;
    }
    Console.Write($"Удалить «{stu.Name}»? (да/нет): ");
    string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
    if (confirm == "да")
    {
        db.DeleteStudent(id);
        Console.WriteLine("Студент удалён.");
    }
    else
    {
        Console.WriteLine("Удаление отменено.");
    }
}

static void ReportsMenu(DatabaseManager db)
{
    string choice;
    do
    {
        Console.WriteLine("--- Отчёты ---");
        Console.WriteLine(" 1 — Студеты по факультетам");
        Console.WriteLine(" 2 — Количество студентов в факультетах");
        Console.WriteLine(" 3 — Средний балл по факультетам");
        Console.WriteLine(" 0 — Назад");
        Console.Write("Ваш выбор: ");
        choice = Console.ReadLine()?.Trim() ?? "";
        switch (choice)
        {
            case "1": Report1_StudentsWithFacs(db); break;
            case "2": Report2_CountByFac(db); break;
            case "3": Report3_AvgGpaByFac(db); break;
            case "0": break;
            default: Console.WriteLine("Неверный пункт."); break;
        }
        Console.WriteLine();
    }
    while (choice != "0");
}

// ─────── Отчёт 1 ───────
static void Report1_StudentsWithFacs(DatabaseManager db)
{
    new ReportBuilder(db)
    .Query(@"SELECT d.stu_name, fac.fac_name, d.stu_commits
        FROM stu d
        JOIN fac ON d.fac_id = fac.fac_id
        ORDER BY d.stu_name")
    .Title("Студенты по факультетам")
    .Header("Имя", "Факультет", "Средний балл")
    .ColumnWidths(20, 15, 10)
    .Footer("Всего записей") // [ГРУППА В] итоговая строка
    .Print();
}
// ─────── Отчёт 2 ───
static void Report2_CountByFac(DatabaseManager db)
{
    new ReportBuilder(db)
    .Query(@"SELECT fac.fac_name, COUNT(*) AS cnt
        FROM stu
        JOIN fac ON stu.fac_id = fac.fac_id
        GROUP BY fac.fac_name
        ORDER BY fac.fac_name")
    .Title("Количество студентов по факультетам")
    .Header("Факультет", "Кол-во")
    .ColumnWidths(20, 10)
    .Print();
}
// ─────── Отчёт 3 ───────
static void Report3_AvgGpaByFac(DatabaseManager db)
{
    new ReportBuilder(db)
    .Query(@"SELECT fac.fac_name,
        ROUND(AVG(stu.stu_gpa), 1) AS avg_gpa
        FROM stu
        JOIN fac ON stu.fac_id = fac.fac_id
        GROUP BY fac.fac_name
        ORDER BY avg_gpa DESC")
    .Title("Средний балл по факультетам")
    .Header("Факультет", "Средний балл")
    .ColumnWidths(20, 20)
    .Print();
}