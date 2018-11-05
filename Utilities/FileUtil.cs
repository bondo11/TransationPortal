using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace translate_spa.Utilities
{
    public static class FileUtil
    {
        static byte[] ObjectToByteArray(this object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using(MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}