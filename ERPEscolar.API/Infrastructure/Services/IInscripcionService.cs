
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPEscolar.DTOs.ControlEscolar;

namespace ERPEscolar.Infrastructure.Services
{
    public interface IInscripcionService
    {
        Task<InscripcionDto> CreateInscripcionAsync(CreateInscripcionDto request);
        Task<InscripcionDto> GetByIdAsync(int id);
        Task<List<InscripcionDto>> GetAllAsync();
        Task<List<InscripcionDto>> GetByAlumnoIdAsync(int alumnoId);
        Task<List<InscripcionDto>> GetByGrupoIdAsync(int grupoId);
        Task DeleteAsync(int id);
    }
}
