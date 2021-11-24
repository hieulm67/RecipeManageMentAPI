using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using RecipeManagementBE.Constant;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Repository;
using RecipeManagementBE.Response.Exception;

namespace RecipeManagementBE.Service.Impl {
    public class LogService<T> : BaseService, ILogService<T> {
        private readonly ILogRepository _logRepository;

        private readonly IAccountRepository _accountRepository;

        public LogService(ILogRepository logRepository, IHttpContextAccessor httpContextAccessor,
            IAccountRepository accountRepository) : base(httpContextAccessor, accountRepository) {
            _logRepository = logRepository;
            _accountRepository = accountRepository;
        }

        public bool WriteLogCreateAccount(Account accountAdded) {
            var currentAccount = GetCurrentAccount();

            var content = $"Account \"{currentAccount.FullName}\" with UID: {currentAccount.UID} and Role: {currentAccount.Role.Name}, " +
                          $"had created account \"{accountAdded.FullName}\" with UID: {accountAdded.UID} and Role: {accountAdded.Role.Name}";

            if (accountAdded.Role.Code != RoleConstants.ADMIN_CODE) {
                content = $"Account \"{currentAccount.FullName}\" with UID: \"{currentAccount.UID}\" and Role: \"{currentAccount.Role.Name}\", " +
                          $"had created account \"{accountAdded.FullName}\" with UID: \"{accountAdded.UID}\" and Role: \"{accountAdded.Role.Name}\" of Brand: \"{accountAdded.Employee.Brand.Name}\"";
            }
            
            var log = new Log {
                Content = content,
                LogTime = DateTime.Now,
                Type = LogTypeConstants.CREATE_LOG
            };

            _logRepository.Add(log);
            // _logRepository.SaveChanges();
            return true;
        }

        public bool WriteLogUpdateAccount(Account accountUpdated) {
            var currentAccount = GetCurrentAccount();
            
            var content = $"Account \"{currentAccount.FullName}\" with UID: {currentAccount.UID} and Role: {currentAccount.Role.Name}, " +
                          $"had updated account \"{accountUpdated.FullName}\" with UID: {accountUpdated.UID} and Role: {accountUpdated.Role.Name}";

            if (accountUpdated.Role.Code != RoleConstants.ADMIN_CODE) {
                content = $"Account \"{currentAccount.FullName}\" with UID: \"{currentAccount.UID}\" and Role: \"{currentAccount.Role.Name}\", " +
                          $"had updated account \"{accountUpdated.FullName}\" with UID: \"{accountUpdated.UID}\" and Role: \"{accountUpdated.Role.Name}\" " +
                          $"of Brand: \"{accountUpdated.Employee.Brand.Name}\"";
            }
            
            var log = new Log {
                Content = content,
                LogTime = DateTime.Now,
                Type = LogTypeConstants.UPDATE_LOG
            };

            _logRepository.Add(log);
            // _logRepository.SaveChanges();
            return true;
        }

        public bool WriteLogCreate(T itemAdded) {
            var type = itemAdded.GetType().Name;
            
            var currentAccount = GetCurrentAccount();

            var log = new Log {
                Content =
                    $"Account \"{currentAccount.FullName}\" with UID: \"{currentAccount.UID}\" and Role: \"{currentAccount.Role.Name}\", " +
                    $"had added {type} {itemAdded.ToString()}",
                LogTime = DateTime.Now,
                Type = LogTypeConstants.CREATE_LOG
            };

            _logRepository.Add(log);
            // _logRepository.SaveChanges();
            return true;
        }

        public bool WriteLogUpdate(T itemUpdated) {
            var type = itemUpdated.GetType().Name;
            
            var currentAccount = GetCurrentAccount();

            var log = new Log {
                Content =
                    $"Account \"{currentAccount.FullName}\" with UID: \"{currentAccount.UID}\" and Role: \"{currentAccount.Role.Name}\", " +
                    $"had updated {type} {itemUpdated.ToString()}",
                LogTime = DateTime.Now,
                Type = LogTypeConstants.UPDATE_LOG
            };

            _logRepository.Add(log);
            // _logRepository.SaveChanges();
            return true;
        }

        public bool WriteLogDelete(T itemDeleted) {
            var type = itemDeleted.GetType().Name;

            var currentAccount = GetCurrentAccount();

            var log = new Log {
                Content =
                    $"Account \"{currentAccount.FullName}\" with UID: \"{currentAccount.UID}\" and Role: \"{currentAccount.Role.Name}\", " +
                    $"had deleted {type} {itemDeleted.ToString()}",
                LogTime = DateTime.Now,
                Type = LogTypeConstants.DELETE_LOG
            };

            _logRepository.Add(log);
            // _logRepository.SaveChanges();
            return true;
        }

        private Account GetCurrentAccount() {
            var jwtToken = GetToken();

            var uid = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.NameId)?.Value;

            if (uid == null) {
                throw new BusinessException(ExceptionCodeMapping.UNAUTHORIZED,
                    new ExceptionParams {Params = new[] {"uid"}});
            }

            return _accountRepository.QueryHelper()
                .Include(account => account.Role)
                .GetOne(account => !account.IsDeleted && account.UID == uid);
        }
    }
}