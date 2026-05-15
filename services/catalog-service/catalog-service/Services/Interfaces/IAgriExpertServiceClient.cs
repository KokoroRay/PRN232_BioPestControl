namespace catalog_service.Services.Interfaces
{
    public interface IAgriExpertServiceClient
    {
        Task<string?> GetChemicalNameAsync(int chemicalProfileId);
        Task<bool> ExistsChemicalProfileAsync(int chemicalProfileId);
    }
}
