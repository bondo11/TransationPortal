using Microsoft.AspNetCore.Http;

namespace translate_spa.Models.ResponseModels
{
    public class ErrorResponse : ResponseModel
    {
        public ErrorResponse(string response = null, string responseText = null,
            int statusCode = StatusCodes.Status400BadRequest, string redirectUrl = null)
        {
            this.StatusCode = statusCode;
            this.Response = response;
            this.ResponseText = responseText;
            this.RedirectUrl = redirectUrl;
            this.Redirect = !string.IsNullOrWhiteSpace(redirectUrl);
        }
    }
}