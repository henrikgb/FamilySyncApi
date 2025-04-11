using FamilySyncApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilySyncApi.Services.Interfaces;

public interface ICalendarScheduleService
{
    Task<List<ScheduleItem>> GetScheduleAsync();
    Task SaveScheduleAsync(List<ScheduleItem> schedule);
}
