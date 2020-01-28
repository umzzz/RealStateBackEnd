using System.IO;
using System.Threading.Tasks;

namespace RealStateAPI.Service
{
    public interface IS3Service
    {
        Task<MemoryStream> getObject(string s3Key);

        Task uploadFile(string fileName, string filepath);
    }
}