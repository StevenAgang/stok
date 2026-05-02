namespace stok.Repository.Interace.EmailService
{
    public interface IEmailService
    {
        Task SendMail(string rawMail);
        string BuildRawMail(string to, string subject, string body);
        string RecoveryAccountMail(string resetLink);
        string WelcomeUserMail();
    }
}
