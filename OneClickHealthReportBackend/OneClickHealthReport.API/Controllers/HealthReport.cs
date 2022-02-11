using Microsoft.AspNetCore.Mvc;

namespace OneClickHealthReport.API.Controllers
{
    [Route("health_report")]
    [ApiController]
    public class HealthReport : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Report([FromBody] ViewModels.HealthReport report_param)
        {
            var health_report_service = new Services.HealthReport(report_param.key, report_param.auth_code);
            var report_param_formatted = new Models.HealthReport();
            report_param_formatted.adcode = report_param.adcode;
            report_param_formatted.nation = report_param.nation;
            report_param_formatted.province = report_param.province;
            report_param_formatted.city = report_param.city;
            report_param_formatted.addr = report_param.addr;
            report_param_formatted.lat = report_param.lat;
            report_param_formatted.lng = report_param.lng;
            report_param_formatted.vaccine_count = report_param.vaccine_count + 1;  // 选项id比真实值大1
            await health_report_service.SubmitHealthReport(report_param_formatted);
            return Ok();
        }
    }
}
