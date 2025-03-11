namespace TestProject1.Basics;

public interface IRepository<TEntity>
{
    void Save(TEntity entity);
}