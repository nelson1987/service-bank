namespace TestProject1.Basics;

public class BasicEndpoint
{
    private readonly IRepository<Entity> _repository;

    public BasicEndpoint(IRepository<Entity> repository)
    {
        _repository = repository;
    }

    public BasicResponse Post(BasicRequest request)
    {
        Entity entity = request;
        _repository.Save(entity);
        return new BasicResponse(entity.Id);
    }
}