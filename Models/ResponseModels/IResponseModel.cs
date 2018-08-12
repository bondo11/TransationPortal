namespace translate_spa.Models.ResponseModels
{
    public interface IResponseModel
    {
        int StatusCode { get; set; }
        string Response { get; set; }
        string ResponseText { get; set; }
        bool Redirect { get; set; }
        string RedirectUrl { get; set; }
    }
}