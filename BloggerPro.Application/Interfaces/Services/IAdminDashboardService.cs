using BloggerPro.Application.DTOs.Dashboard;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface IAdminDashboardService
    {
        Task<DataResult<DashboardStatsDto>> GetDashboardStatsAsync();
    }

}
