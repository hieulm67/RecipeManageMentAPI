namespace RecipeManagementBE.Constant {
    public static class ApiPathURL {

        public const string API_V1_PATH = "/api/v1/";

        public const string ACCOUNT_API_PATH = API_V1_PATH + "account";
        public const string ADMIN_API_PATH = API_V1_PATH + "admins";
        public const string EMPLOYEE_API_PATH = API_V1_PATH + "employees";
        public const string BRAND_API_PATH = API_V1_PATH + "brands";
        public const string CATEGORY_BY_BRAND_API_PATH = BRAND_API_PATH + "/categories";
        public const string CATEGORY_API_PATH = API_V1_PATH + "categories";
        public const string DISH_API_PATH = API_V1_PATH + "dishes";
        public const string DISH_BY_CATEGORY_API_PATH = CATEGORY_API_PATH + "/dishes";
        public const string DISH_BY_BRAND_API_PATH = CATEGORY_BY_BRAND_API_PATH + "/dishes";
        public const string RECIPE_BY_DISH_API_PATH = DISH_API_PATH + "/recipes";
        public const string RECIPE_API_PATH = API_V1_PATH + "recipes";
        public const string INGREDIENT_API_PATH = API_V1_PATH + "ingredients";
        public const string PROCESSING_STEP_API_PATH = API_V1_PATH + "processing-step";
        public const string PROCESSING_STEP_BY_RECIPE_API_PATH = RECIPE_API_PATH + "/processing-step";
        public const string LOG_API_PATH = API_V1_PATH + "log";
        public const string NOTIFICATION_API_PATH = API_V1_PATH + "notifications";
        public const string QA_API_PATH = API_V1_PATH + "qas";
        public const string QA_BY_RECIPE_API_PATH = RECIPE_BY_DISH_API_PATH + "/qas";
        public const string TOOL_API_PATH = API_V1_PATH + "tools";
    }
}