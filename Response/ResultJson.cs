using System;

namespace RecipeManagementBE.Response {
    public class ResultJson<T> {

        public T Data { get; set; }

        public string Message { get; set; } = "Success";
    }
}