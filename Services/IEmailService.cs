namespace servicer.API.Services
{
    public interface IEmailService
    {
        void SendEmailMessage(string toAddress, string subject, string messageHtml, string messageTxt);
    }
}