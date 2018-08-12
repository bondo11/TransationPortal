namespace translate_spa.MongoDB.Interfaces
{
    public interface IDbBuilder
    {
        string GetDatabaseName();

        string GetConnectionString();
    }
}