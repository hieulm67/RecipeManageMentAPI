using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecipeManagementBE.Constant;
using RecipeManagementBE.Firebase;
using RecipeManagementBE.Repository;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Response.Exception;
using FirebaseAuth = FirebaseAdmin.Auth.FirebaseAuth;

namespace RecipeManagementBE.Service.Impl {
    public class FirebaseService : IFirebaseService {

        private readonly FirebaseMetadata _firebaseMetadata;

        private readonly ILogger<FirebaseService> _logger;

        public FirebaseService(IOptions<FirebaseMetadata> firebaseMetadata,
            ILogger<FirebaseService> logger) {
            _firebaseMetadata = firebaseMetadata.Value;
            FirebaseApp.Create(new AppOptions {
                Credential = GoogleCredential.FromFile("./staffmate.json"),
            });
            _logger = logger;
        }

        public async Task<string> UploadImage(UploadImageDTO dto) {
            var imageBase64String = dto.ImageBase64String ?? string.Empty;
            var fileName = dto.FileName ?? string.Empty;

            if (string.IsNullOrWhiteSpace(imageBase64String)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing image string base 64, missing required field exception throw",
                                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.ITEM_FIELD_REQUIRED_MISSING, new ExceptionParams{Params = new[]{"imageBase64String"}});
            }
            
            if (string.IsNullOrWhiteSpace(fileName)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing image name, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.ITEM_FIELD_REQUIRED_MISSING, new ExceptionParams{Params = new[]{"imageFileName"}});
            }
            
            var bytes = Convert.FromBase64String(imageBase64String);

            await using (var stream = new MemoryStream(bytes)) {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseMetadata.ApiKey));
                var user = await auth.SignInWithEmailAndPasswordAsync(_firebaseMetadata.AuthEmail,
                    _firebaseMetadata.AuthPassword);

                var task = new FirebaseStorage(_firebaseMetadata.Bucket, new FirebaseStorageOptions {
                        AuthTokenAsyncFactory = () => Task.FromResult(user.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child("images")
                    .Child(fileName)
                    .PutAsync(stream);
                
                task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");
                return await task;
            }
        }

        public bool AuthenticationFirebase(string email, string password, string uid) {
            try {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseMetadata.ApiKey));
                var user = auth.SignInWithEmailAndPasswordAsync(email,
                    password);

                return user.Result.User.LocalId.Equals(uid);
            }
            catch (Exception ex) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: {Message}",
                    DateTime.Now ,Constants.APPLICATION_NAME, ex.Message);
                return false;
            }
        }
        
        public async Task DeleteAccountFirebase(string uid) {
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
        }
        
        public async Task DeleteMultipleAccountFirebase(List<string> uids) {
            await FirebaseAuth.DefaultInstance.DeleteUsersAsync(uids);
        }
    }
}
