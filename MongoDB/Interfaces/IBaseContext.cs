using translate_spa.Models;
using translate_spa.Models.Interfaces;

namespace translate_spa.MongoDB.Interfaces
{
    public interface IBaseContext
    {
        IRepository<T> ResolveRepository<T>()
        where T : IEntity;
    }
}