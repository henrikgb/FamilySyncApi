using FamilySyncApi.Models;
using FamilySyncApi.Repositories.Interfaces;
using FamilySyncApi.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilySyncApi.Services.Implementations;

public class CalendarScheduleService : ICalendarScheduleService
{
    private readonly IBlobStorageRepository<ScheduleItem> _repository;
    private const string BlobName = "CalendarSchedule.json";

    public CalendarScheduleService(IBlobStorageRepository<ScheduleItem> repository)
    {
        _repository = repository;
    }

    public Task<List<ScheduleItem>> GetScheduleAsync()
    {
        return _repository.GetAsync(BlobName);
    }

    public Task SaveScheduleAsync(List<ScheduleItem> schedule)
    {
        return _repository.SaveAsync(BlobName, schedule);
    }
}
