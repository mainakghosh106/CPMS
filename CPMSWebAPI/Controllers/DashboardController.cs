using CPMSWebAPI.Data;
using CPMSWebAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CPMSWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _appdbcontext;
        public DashboardController(AppDbContext appDbcontext)
        {
            _appdbcontext = appDbcontext;
        }
        [HttpGet("GetDashboardSummary")]
        public async Task<DashboardSummaryDto> GetDashboardSummary(DateTime? fromDate = null, DateTime? toDate = null)
        {
            int userId= Convert.ToInt32(User.FindFirst("UserId")?.Value);
            var parameters = new[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value),
                new SqlParameter("@ToDate", toDate ?? (object)DBNull.Value)
            };

            var result = await _appdbcontext.DashboardSummaryDto.FromSqlRaw("EXEC usp_GetDashboardSummary @UserId, @FromDate, @ToDate", parameters).FirstOrDefaultAsync();
            return result;
        }

    }
}
