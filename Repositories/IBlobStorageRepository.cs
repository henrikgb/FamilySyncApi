using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilySyncApi.Repositories;

public interface IBlobStorageRepository<T>
{
    Task<List<T>> GetAsync(string blobName);
    Task SaveAsync(string blobName, List<T> data);
}
