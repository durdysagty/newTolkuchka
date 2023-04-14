namespace newTolkuchka.Services.Interfaces
{
    public interface IPath
    {
        string GetImagesFolder();
        string GetImagePath(string folder, int id, int imageNumber = 0);
        string GetSVGFolder();
        string GetHtmlBodyPath();
        string GetHtmlPinBodyPath();
        string GetHtmlRecoveryBodyPath();
        string GetHtmlNewPinBodyPath();
        string GetHtmlAboutBodyPath(string lang);
        string GetHtmlDeliveryBodyPath(string lang);
        string GetLogo();
    }
}