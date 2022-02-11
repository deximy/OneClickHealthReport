using Microsoft.AspNetCore.SignalR;
using OneClickHealthReport.API.Services;

namespace OneClickHealthReport.API.Hubs
{
    public interface ILoginClient
    {
        Task ReceiveAuthCode(string auth_code);
        Task ReceiveKey(string key);
        Task ReceiveQrCode(string img_base64);
    }

    public class Login : Hub<ILoginClient>
    {
        public readonly WeComLogin wecom_qr_code_service_;

        public Login(WeComLogin wecom_qr_code_service)
        {
            wecom_qr_code_service_ = wecom_qr_code_service;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            while (true)
            {
                var qr_code = await wecom_qr_code_service_.GetQrCode();
                await Clients.Caller.ReceiveQrCode(Convert.ToBase64String(qr_code));
                var scan_result = await wecom_qr_code_service_.GetScanResult();
                if (scan_result == true)
                {
                    await Clients.Caller.ReceiveAuthCode(wecom_qr_code_service_.GetAuthCode());
                    await Clients.Caller.ReceiveKey(wecom_qr_code_service_.GetKey());
                    return;
                }
            }
        }
    }
}
