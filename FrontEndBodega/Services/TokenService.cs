using Microsoft.JSInterop;

namespace FrontEndBodega.Services
{
    public class TokenService
    {
        private readonly IJSRuntime _js;

        public TokenService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<string> GetTokenAsync()
        {
            return await _js.InvokeAsync<string>("getToken");
        }
    }
}
