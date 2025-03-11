namespace TestProject1.Basics;

#region REPR

public record BasicRequest(string Name)
{
    public static implicit operator Entity(BasicRequest source)
    {
        return new Entity(source.Name);
    }
}

#endregion