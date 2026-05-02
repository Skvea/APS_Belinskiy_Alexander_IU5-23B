using Microsoft.Data.Sqlite;

class DatabaseManager
{
    private string _connectionString;

    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    public void InitializeDatabase(string facCsvPath, string stuCsvPath)
    {
        CreateTables();

        if (GetAllFaculties().Count == 0 && File.Exists(facCsvPath))
        {
            ImportFacultiesFromCsv(facCsvPath);
            Console.WriteLine($"[OK] Загружены студенты из {facCsvPath}");
        }

        if (GetAllStudents().Count == 0 && File.Exists(stuCsvPath))
        {
            ImportStudentsFromCsv(stuCsvPath);
            Console.WriteLine($"[OK] Загружены студенты из {stuCsvPath}");
        }
    }

    private void CreateTables()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS fac (
            fac_id INTEGER PRIMARY KEY AUTOINCREMENT,
            fac_name TEXT NOT NULL
        );
        CREATE TABLE IF NOT EXISTS stu (
            stu_id INTEGER PRIMARY KEY AUTOINCREMENT,
            fac_id INTEGER NOT NULL,
            stu_name TEXT NOT NULL,
            stu_gpa DOUBLE NOT NULL,
            FOREIGN KEY (fac_id) REFERENCES fac(fac_id)
        );";
        cmd.ExecuteNonQuery();
    }

    private void ImportFacultiesFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 2) continue;

            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "INSERT INTO fac (fac_id, fac_name) VALUES (@id, @name)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@name", parts[1]);
            cmd.ExecuteNonQuery();
        }
    }

    private void ImportStudentsFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 4) continue;

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            INSERT INTO stu (stu_id, fac_id, stu_name, stu_gpa)
            VALUES (@id, @facId, @name, @gpa)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@facId", int.Parse(parts[1]));
            cmd.Parameters.AddWithValue("@name", parts[2]);
            cmd.Parameters.AddWithValue("@gpa", double.Parse(parts[3]));
            cmd.ExecuteNonQuery();
        }
    }

    public List<Faculty> GetAllFaculties()
    {
        var result = new List<Faculty>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT fac_id, fac_name FROM fac ORDER BY fac_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Faculty(
            reader.GetInt32(0),
            reader.GetString(1)));
        }
        return result;
    }

    public List<Student> GetAllStudents()
    {
        var result = new List<Student>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText =
        "SELECT stu_id, fac_id, stu_name, stu_gpa FROM stu ORDER BY stu_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Student(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetString(2),
            reader.GetDouble(3)));
        }
        return result;
    }

    public Student GetStudentById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText =
        "SELECT stu_id, fac_id, stu_name, stu_gpa FROM stu WHERE stu_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Student(
            reader.GetInt32(0), reader.GetInt32(1),
            reader.GetString(2), reader.GetDouble(3));
        }
        return null;
    }

    public void AddStudent(Student stu)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
        INSERT INTO stu (fac_id, stu_name, stu_gpa)
        VALUES (@facId, @name, @gpa)";
        cmd.Parameters.AddWithValue("@facId", stu.FacultyId);
        cmd.Parameters.AddWithValue("@name", stu.Name);
        cmd.Parameters.AddWithValue("@gpa", stu.Gpa);
        cmd.ExecuteNonQuery();
    }

    public void UpdateStudent(Student stu)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
        UPDATE stu
        SET fac_id = @facId, stu_name = @name, stu_gpa = @gpa
        WHERE stu_id = @id";
        cmd.Parameters.AddWithValue("@id", stu.Id);
        cmd.Parameters.AddWithValue("@facId", stu.FacultyId);
        cmd.Parameters.AddWithValue("@name", stu.Name);
        cmd.Parameters.AddWithValue("@gpa", stu.Gpa);
        cmd.ExecuteNonQuery();
    }

    public void DeleteStudent(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM dev WHERE stu_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();

        string[] columns = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
            columns[i] = reader.GetName(i);

        var rows = new List<string[]>();
        while (reader.Read())
        {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i)?.ToString() ?? "";
            rows.Add(row);
        }
        return (columns, rows);
    }
}