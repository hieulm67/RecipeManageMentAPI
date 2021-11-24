namespace RecipeManagementBE.Response.Exception {
    public class BusinessException : System.Exception {
        public BusinessException(string errorCode, ExceptionParams exceptionParams) : this(null, null, exceptionParams,
            errorCode) {
        }

        public BusinessException(string errorCode, string detail, ExceptionParams exceptionParams) : this(null, detail,
            exceptionParams,
            errorCode) {
        }

        public BusinessException(string type, string detail, ExceptionParams exceptionParams, string errorCode) :
            base(detail) {
            this.Type = type;
            this.Detail = detail;
            this.ExceptionParams = exceptionParams;
            this.ErrorCode = errorCode;
        }

        public string Type { get; set; }

        public string Detail { get; set; }

        public ExceptionParams ExceptionParams { get; set; }

        public string ErrorCode { get; set; }
    }
}