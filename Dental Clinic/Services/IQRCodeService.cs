namespace Dental_Clinic.Services
{
    public interface IQRCodeService
    {
        string GenerateQRCode(string url);
    }
}
