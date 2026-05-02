class Student
{
    public int Id { get; set; }
    public int FacultyId { get; set; }
    public string Name { get; set; }

    private double _gpa;

    public double Gpa
    {
        get => _gpa;
        set
        {
            if (value < 2 || value > 5)
                throw new ArgumentException("Средний балл может только входить в [2, 5]");
            _gpa = value;
        }
    }

    public Student(int id, int facultyId, string name, double gpa)
    {
        Id = id;
        FacultyId = facultyId;
        Name = name;
        Gpa = gpa;
    }

    public Student() : this(0, 0, "", 0) { }

    public override string ToString()
    {
        return $"[{Id}] {Name}, факультет: {FacultyId}, средний балл: {Gpa}";
    }

}