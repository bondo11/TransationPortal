using System.Collections.Generic;

namespace translate_spa.Models.Interfaces
{
    public interface IEntity
    {
        string Id { get; set; }
        string Key { get; set; }
        string Da { get; set; }
        string En { get; set; }
        string Sv { get; set; }
        string Nb { get; set; }
        string Branch { get; set; }

        Dictionary<string, string> GetProperties();
    }
}