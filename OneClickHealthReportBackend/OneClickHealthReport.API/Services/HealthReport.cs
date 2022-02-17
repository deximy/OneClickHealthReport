using System.Text.Json;

namespace OneClickHealthReport.API.Services
{
    internal class Response
    {
        public Head? head { get; set; }
        public Body? body { get; set; }
    }

    internal class Head
    {
        public int? ret { get; set; }
    }

    internal class Body
    {
        public List<FormInfo>? form_items { get; set; }
        public object? autofill_form_reply { get; set; }
    }

    internal class FormInfo
    {
        public string? form_id { get; set; }
        public string? title { get; set; }
    }

    public class HealthReport
    {
        private readonly HttpClient client_ = new HttpClient(
            new HttpClientHandler() {
                AllowAutoRedirect = false,
            }
        );
        private readonly Task init_;

        public HealthReport(string key, string auth_code)
        {
            var response = client_.GetAsync($"https://doc.weixin.qq.com/disklogin/login?product=0&from=4&type=2&redirect_url=&code={auth_code}&wwqrlogin=1&qrcode_key={key}&auth_source=SOURCE_FROM_WEWORK");
            response.GetAwaiter().OnCompleted(
                () => {
                    if (response.Result.Headers.Location?.ToString() == "https://drive.weixin.qq.com/webdisk/index?t=home")
                    {
                        Console.WriteLine(response.Result.Headers);
                        return;
                    }
                    throw new InvalidDataException(response.Result.ToString());
                }
            );
            init_ = response;
        }

        public async Task<string> GetHealthReportFormId()
        {
            Task.WaitAll(init_);
            var response = await client_.GetAsync("https://doc.weixin.qq.com/form/healthformlist");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response>(content);
            if (result?.head?.ret == 0)
            {
                foreach (var item in result?.body?.form_items ?? new List<FormInfo>())
                {
                    if (item.title == "每日健康打卡" && item.form_id != null)
                    {
                        return item.form_id;
                    }
                }
            }
            throw new InvalidDataException(content);
        }

        private async Task<string> GetLastReport(string form_id)
        {
            var response = await client_.GetAsync($"https://doc.weixin.qq.com/form/share?f=json&form_id={form_id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (content == null)
            {
                throw new InvalidDataException(response.ToString());
            }
            var result = JsonSerializer.Deserialize<Response>(content);
            var last_report = result?.body?.autofill_form_reply?.ToString();
            if (last_report == null)
            {
                throw new InvalidDataException(content);
            }
            return last_report;
        }

        public async Task<bool> SubmitHealthReport()
        {
            var form_id = await GetHealthReportFormId();
            var form_reply = await GetLastReport(form_id);
            var request_content = new MultipartFormDataContent();
            var boundary = request_content.Headers?.ContentType?.Parameters.Single(o => o.Name == "boundary");
            boundary!.Value = boundary?.Value?.Replace("\"", string.Empty);
            request_content.Add(new StringContent("2"), "\"type\"");
            request_content.Add(new StringContent("json"), "\"f\"");
            request_content.Add(new StringContent("hb_noticard"), "\"source\"");
            request_content.Add(new StringContent("null"), "\"vcode\"");
            request_content.Add(new StringContent(form_id), "\"form_id\"");
            request_content.Add(new StringContent(form_reply), "\"form_reply\"");

            var response = await client_.PostAsync("https://doc.weixin.qq.com/form/share?f=json", request_content);
            response.EnsureSuccessStatusCode();
            var response_content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response>(response_content);
            if (result?.head?.ret == 0)
            {
                return true;
            }
            throw new InvalidDataException(response_content);
        }

        public async Task<bool> SubmitHealthReport(Models.HealthReport report_param)
        {
            var request_content = new MultipartFormDataContent();
            var boundary = request_content.Headers?.ContentType?.Parameters.Single(o => o.Name == "boundary");
            boundary!.Value = boundary?.Value?.Replace("\"", string.Empty);
            request_content.Add(new StringContent("2"), "\"type\"");
            request_content.Add(new StringContent("json"), "\"f\"");
            request_content.Add(new StringContent("hb_noticard"), "\"source\"");
            request_content.Add(new StringContent("null"), "\"vcode\"");
            var form_id = await GetHealthReportFormId();
            request_content.Add(new StringContent(form_id), "\"form_id\"");
            string form_reply = $"{{\"items\":[{{\"question_id\":1,\"text_reply\":\"{{\\\"module\\\":\\\"geolocation\\\",\\\"type\\\":\\\"ip\\\",\\\"adcode\\\":{report_param.adcode},\\\"nation\\\":\\\"{report_param.nation}\\\",\\\"province\\\":\\\"{report_param.province}\\\",\\\"city\\\":\\\"{report_param.city}\\\",\\\"addr\\\":\\\"{report_param.addr}\\\",\\\"lat\\\":{report_param.lat},\\\"lng\\\":{report_param.lng},\\\"accuracy\\\":10000,\\\"exportText\\\":\\\"{report_param.addr}\\\"}}\",\"option_reply\":[]}},{{\"question_id\":2,\"text_reply\":\"\",\"option_reply\":[1]}},{{\"question_id\":3,\"text_reply\":\"\",\"option_reply\":[2]}},{{\"question_id\":4,\"text_reply\":\"\",\"option_reply\":[2]}},{{\"question_id\":6,\"text_reply\":\"\",\"option_reply\":[2]}},{{\"question_id\":7,\"text_reply\":\"\",\"option_reply\":[1]}},{{\"question_id\":8,\"text_reply\":\"\",\"option_reply\":[5]}},{{\"question_id\":9,\"text_reply\":\"\",\"option_reply\":[2]}},{{\"question_id\":10,\"text_reply\":\"\",\"option_reply\":[{report_param.vaccine_count}]}},{{\"question_id\":11,\"text_reply\":\"\",\"option_reply\":[3]}},{{\"question_id\":12,\"text_reply\":\"\",\"option_reply\":[]}},{{\"question_id\":13,\"text_reply\":\"\",\"option_reply\":[]}}]}}";
            request_content.Add(new StringContent(form_reply), "\"form_reply\"");

            var response = await client_.PostAsync("https://doc.weixin.qq.com/form/share?f=json", request_content);
            response.EnsureSuccessStatusCode();
            var response_content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response>(response_content);
            if (result?.head?.ret == 0)
            {
                return true;
            }
            throw new InvalidDataException(response_content);
        }
    }
}
