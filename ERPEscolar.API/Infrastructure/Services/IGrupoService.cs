
using ERPEscolar.DTOs.ControlEscolar;

namespace ERPEscolar.Infrastructure.Services
{
    public interface IGrupoService
    {
        Task<GrupoDto> CreateGrupoAsync(CreateGrupoDto request);
        Task<GrupoDto> GetByIdAsync(int id);
        // Task<GrupoCompleteDto> GetByIdCompleteAsync(int id); // Lo implementaremos después
        Task<List<GrupoDto>> GetAllAsync();
        Task<GrupoDto> UpdateGrupoAsync(int id, UpdateGrupoDto request);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
    }
}
