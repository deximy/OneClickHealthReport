namespace OneClickHealthReport.API.Models
{
    public class HealthReport
    {
        public string adcode { get; set; } = "320100";
        public string nation { get; set; } = "中国";
        public string province { get; set; } = "江苏省";
        public string city { get; set; } = "南京市";
        public string addr { get; set; } = "江苏省南京市南京邮电大学(仙林校区)";
        public double lat { get; set; } = 32.120847;
        public double lng { get; set; } = 118.922687;
        public int vaccine_count { get; set; } = 2;
    }
}
