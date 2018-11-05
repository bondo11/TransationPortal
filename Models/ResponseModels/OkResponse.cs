using Microsoft.AspNetCore.Http;

namespace translate_spa.Models.ResponseModels
{
    public class OkResponse : ResponseModel
    {
        public OkResponse(string response = null, string responseText = null, int statusCode = StatusCodes.Status200OK,
            string redirectUrl = null)
        {
            this.StatusCode = statusCode;
            this.Response = response;
            this.ResponseText = responseText;
            this.RedirectUrl = redirectUrl;
            this.Redirect = !string.IsNullOrWhiteSpace(redirectUrl);
        }
    }
}