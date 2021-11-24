namespace RecipeManagementBE.Request.Create {
    public class CreateQADTO {

        public long RecipeId { get; set; }
        
        public long QaParentId { get; set; }
        
        public string Content { get; set; }

        public bool IsReply { get; set; }
    }
}