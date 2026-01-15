
using AutoMapper;
using ERPEscolar.API.Models;
using ERPEscolar.DTOs.ControlEscolar;
using ERPEscolar.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPEscolar.API.Core.Exceptions;
using System.Linq;

namespace ERPEscolar.Infrastructure.Services
{
    public class GrupoService : IGrupoService
    {
        private readonly IRepository<Grupo> _grupoRepository;
        private readonly IRepository<CicloEscolar> _cicloRepository;
        private readonly ILogger<GrupoService> _logger;
        private readonly IMapper _mapper;

        public GrupoService(IRepository<Grupo> grupoRepository, IRepository<CicloEscolar> cicloRepository, ILogger<GrupoService> logger, IMapper mapper)
        {
            _grupoRepository = grupoRepository;
            _cicloRepository = cicloRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<GrupoDto> CreateGrupoAsync(CreateGrupoDto request)
        {
            _logger.LogInformation("Iniciando proceso para crear un nuevo grupo: {Nombre}", request.Nombre);

            // 1. Validar que el CicloEscolarId exista
            var cicloExists = await _cicloRepository.GetByIdAsync(request.CicloEscolarId);
            if (cicloExists == null)
            {
                _logger.LogWarning("Intento de crear grupo con CicloEscolarId no existente: {CicloId}", request.CicloEscolarId);
                throw new NotFoundException($"El ciclo escolar con ID {request.CicloEscolarId} no fue encontrado.");
            }

            // 2. Validar que no haya un grupo con el mismo nombre en el mismo ciclo
            var allGrupos = await _grupoRepository.GetAllAsync();
            var grupoExists = allGrupos.Any(g => g.Nombre.Equals(request.Nombre, StringComparison.OrdinalIgnoreCase) && g.CicloEscolarId == request.CicloEscolarId && g.Activo);

            if (grupoExists)
            {
                _logger.LogWarning("Intento de crear un grupo duplicado: {Nombre} en CicloId {CicloId}", request.Nombre, request.CicloEscolarId);
                throw new ConflictException($"Ya existe un grupo con el nombre '{request.Nombre}' en este ciclo escolar.");
            }

            // 3. Mapear DTO a la entidad y crear
            var grupo = _mapper.Map<Grupo>(request);
            grupo.Activo = true;
            grupo.FechaCreacion = DateTime.UtcNow;

            var nuevoGrupo = await _grupoRepository.AddAsync(grupo);
            await _grupoRepository.SaveChangesAsync();

            _logger.LogInformation("Grupo {Nombre} creado exitosamente con ID {Id}", nuevoGrupo.Nombre, nuevoGrupo.Id);

            // 4. Mapear la entidad de vuelta a un DTO para la respuesta
            var grupoDto = _mapper.Map<GrupoDto>(nuevoGrupo);
            grupoDto.CicloEscolarNombre = cicloExists.Nombre;

            return grupoDto;
        }

        public async Task<List<GrupoDto>> GetAllAsync()
        {
            var grupos = await _grupoRepository.GetAllAsync();
            return _mapper.Map<List<GrupoDto>>(grupos.Where(g => g.Activo));
        }

        public async Task<GrupoDto> GetByIdAsync(int id)
        {
            var grupo = await _grupoRepository.GetByIdAsync(id);
            if (grupo == null || !grupo.Activo)
            {
                throw new NotFoundException($"Grupo con ID {id} no encontrado.");
            }
            return _mapper.Map<GrupoDto>(grupo);
        }

        public async Task RestoreAsync(int id)
        {
            var grupo = await _grupoRepository.GetByIdAsync(id, includeDeleted: true);
            if (grupo == null)
            {
                throw new NotFoundException($"Grupo con ID {id} no encontrado.");
            }

            if (grupo.Activo)
            {
                throw new ConflictException("El grupo ya está activo.");
            }

            grupo.Activo = true;
            grupo.FechaActualizacion = DateTime.UtcNow;
            await _grupoRepository.UpdateAsync(grupo);
            await _grupoRepository.SaveChangesAsync();
            _logger.LogInformation("Grupo con ID {Id} ha sido reactivado.", id);
        }

        public async Task SoftDeleteAsync(int id)
        {
            var grupo = await _grupoRepository.GetByIdAsync(id);
            if (grupo == null)
            {
                throw new NotFoundException($"Grupo con ID {id} no encontrado.");
            }

            grupo.Activo = false;
            grupo.FechaActualizacion = DateTime.UtcNow;
            await _grupoRepository.UpdateAsync(grupo);
            await _grupoRepository.SaveChangesAsync();
             _logger.LogInformation("Grupo con ID {Id} ha sido desactivado (soft delete).", id);
        }

        public async Task<GrupoDto> UpdateGrupoAsync(int id, UpdateGrupoDto request)
        {
             var grupo = await _grupoRepository.GetByIdAsync(id);
            if (grupo == null || !grupo.Activo)
            {
                throw new NotFoundException($"Grupo con ID {id} no encontrado.");
            }

            _mapper.Map(request, grupo);
            grupo.FechaActualizacion = DateTime.UtcNow;

            await _grupoRepository.UpdateAsync(grupo);
            await _grupoRepository.SaveChangesAsync();
            
            _logger.LogInformation("Grupo con ID {Id} actualizado.", id);

            return _mapper.Map<GrupoDto>(grupo);
        }
    }
}
