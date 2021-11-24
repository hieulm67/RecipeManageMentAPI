using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Service {
    public interface ILogService<T> {

        bool WriteLogCreateAccount(Account accountAdded);
        
        bool WriteLogUpdateAccount(Account accountUpdated);
        
        bool WriteLogCreate(T itemAdded);
        
        bool WriteLogUpdate(T itemUpdated);
        
        bool WriteLogDelete(T itemDeleted);
    }
}