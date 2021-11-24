namespace RecipeManagementBE.Service {
    public interface IMailService {

        void SendMail(string email, string subject, string content);
    }
}