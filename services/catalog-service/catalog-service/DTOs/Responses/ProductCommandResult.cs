namespace catalog_service.DTOs.Responses
{
    public enum ProductCommandError
    {
        None,
        ProductNotFound,
        CategoryNotFound,
        ChemicalProfileNotFound,
        DuplicateSku
    }

    public class ProductCommandResult
    {
        public bool Success { get; init; }
        public ProductCommandError Error { get; init; } = ProductCommandError.None;
        public ProductResponse? Data { get; init; }

        public static ProductCommandResult Ok(ProductResponse? data = null) =>
            new() { Success = true, Data = data };

        public static ProductCommandResult Fail(ProductCommandError error) =>
            new() { Success = false, Error = error };
    }
}
