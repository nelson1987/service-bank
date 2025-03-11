namespace TestProject1.Basics;

public class Entity
{

    public Guid Id { get; }
    public string Name { get; }

    public Entity(string name)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentException(nameof(name));
    }
}