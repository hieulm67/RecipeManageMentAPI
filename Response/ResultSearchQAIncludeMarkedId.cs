using System.Collections.Generic;
using RecipeManagementBE.DTO;

namespace RecipeManagementBE.Response {
    public class ResultSearchQAIncludeMarkedId {

        public List<QaDTO> ListQA { get; set; }

        public long IdMarked { get; set; }

        public bool IsChild { get; set; }

        public long ParentId { get; set; }
    }
}