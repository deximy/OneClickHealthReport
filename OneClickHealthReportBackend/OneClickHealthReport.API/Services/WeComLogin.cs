using System.Text.Json;

namespace OneClickHealthReport.API.Services
{
    internal class Key
    {
        public KeyData? data { get; set; }
    }

    internal class KeyData
    {
        public string? qrcode_key { get; set; }
    }

    internal class Scan
    {
        public ScanData? data { get; set; }
        public ScanResult? result { get; set; }
    }

    internal class ScanData
    {
        public string? status { get; set; }
        public string? auth_code { get; set; }
        public string? auth_source { get; set; }
        public int? corp_id { get; set; }
        public int? code_type { get; set; }
        public string? clientip { get; set; }
        public string? confirm_clientip { get; set; }
    }

    internal class ScanResult
    {
        public int? errCode { get; set; }
        public string? humanMessage { get; set; }
    }

    public class WeComLogin
    {
        private readonly HttpClient client_ = new HttpClient();
        private Task<string> key_;
        private string? auth_code_;

        public WeComLogin()
        {
            key_ = RefreshKey();
        }

        public async Task<string> RefreshKey()
        {
            // TODO: Optimize parameters.
            var response = await client_.GetAsync("https://work.weixin.qq.com/wework_admin/wwqrlogin/get_key");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Key>(content);
            if (result == null || result.data == null || result.data.qrcode_key == null)
            {
                throw new InvalidDataException(content);
            }
            return result.data.qrcode_key;
        }

        public async Task<byte[]> GetQrCode()
        {
            var response = await client_.GetAsync($"https://work.weixin.qq.com/wwqrlogin/qrcode/{key_.Result}?login_type=login_netdisk_wedoc");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            return content;
        }

        public async Task<bool> GetScanResult()
        {
            while (true)
            {
                var response = await client_.GetAsync($"https://work.weixin.qq.com/wework_admin/wwqrlogin/check?qrcode_key={key_.Result}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var scan = JsonSerializer.Deserialize<Scan>(content);
                if (scan == null)
                {
                    throw new InvalidDataException(content);
                }

                if (scan.result != null && scan.data == null)
                {
                    if (scan.result.errCode == -31024)
                    {
                        key_ = RefreshKey();
                        return false;
                    }
                }

                if (scan.data != null && scan.result == null)
                {
                    if (scan.data.status == "QRCODE_SCAN_SUCC")
                    {
                        auth_code_ = scan.data.auth_code;
                        return true;
                    }

                    continue;
                }

                throw new InvalidDataException(scan.ToString());
            }
        }

        public string GetAuthCode()
        {
            if (auth_code_ == null)
            {
                throw new InvalidOperationException();
            }
            return auth_code_;
        }

        public string GetKey()
        {
            return key_.Result;
        }
    }
}
