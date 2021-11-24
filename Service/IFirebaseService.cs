using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RecipeManagementBE.Request.Create;

namespace RecipeManagementBE.Service {
    public interface IFirebaseService {

        Task<string> UploadImage(UploadImageDTO dto);

        bool AuthenticationFirebase(string email, string password, string uid);

        Task DeleteAccountFirebase(string uid);

        Task DeleteMultipleAccountFirebase(List<string> uids);
    }
}