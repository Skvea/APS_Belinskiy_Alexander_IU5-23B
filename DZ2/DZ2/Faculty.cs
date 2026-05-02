class Faculty 
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Faculty(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public Faculty() : this(0,""){}

    public override string ToString()
    {
        return $"[{Id}] {Name}";
    }

}