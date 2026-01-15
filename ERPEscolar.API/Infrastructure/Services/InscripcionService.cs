
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ERPEscolar.API.Models;
using ERPEscolar.DTOs.ControlEscolar;
using ERPEscolar.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using ERPEscolar.API.Core.Exceptions;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace ERPEscolar.Infrastructure.Services
{
    public class InscripcionService : IInscripcionService
    {
        private readonly IRepository<Inscripcion> _inscripcionRepository;
        private readonly IRepository<Alumno> _alumnoRepository;
        private readonly IRepository<Grupo> _grupoRepository;
        private readonly ILogger<InscripcionService> _logger;
        private readonly IMapper _mapper;

        public InscripcionService(
            IRepository<Inscripcion> inscripcionRepository, 
            IRepository<Alumno> alumnoRepository, 
            IRepository<Grupo> grupoRepository, 
            ILogger<InscripcionService> logger, 
            IMapper mapper)
        {
            _inscripcionRepository = inscripcionRepository;
            _alumnoRepository = alumnoRepository;
            _grupoRepository = grupoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<InscripcionDto> CreateInscripcionAsync(CreateInscripcionDto request)
        {
            _logger.LogInformation("Iniciando proceso de inscripción para AlumnoId: {AlumnoId} en GrupoId: {GrupoId}", request.AlumnoId, request.GrupoId);

            // 1. Validar que el alumno y el grupo existan y estén activos
            var alumno = await _alumnoRepository.GetByIdAsync(request.AlumnoId);
            if (alumno == null || !alumno.Activo)
            {
                throw new NotFoundException($"El alumno con ID {request.AlumnoId} no existe o no está activo.");
            }

            var grupo = await _grupoRepository.GetByIdAsync(request.GrupoId);
            if (grupo == null || !grupo.Activo)
            {
                throw new NotFoundException($"El grupo con ID {request.GrupoId} no existe o no está activo.");
            }

            // 2. Validar que el alumno no esté ya inscrito en el mismo grupo
            var inscripciones = await _inscripcionRepository.GetAllAsync();
            var yaInscrito = inscripciones.Any(i => i.AlumnoId == request.AlumnoId && i.GrupoId == request.GrupoId && i.Activo);
            if (yaInscrito)
            {
                throw new ConflictException("El alumno ya está inscrito en este grupo.");
            }
            
            // 3. Validar capacidad del grupo
            var inscritosEnGrupo = inscripciones.Count(i => i.GrupoId == request.GrupoId && i.Activo);
            if (inscritosEnGrupo >= grupo.CapacidadMaxima)
            {
                throw new ConflictException($"El grupo '{grupo.Nombre}' ha alcanzado su capacidad máxima de {grupo.CapacidadMaxima} alumnos.");
            }

            // 4. Crear y guardar la inscripción
            var inscripcion = _mapper.Map<Inscripcion>(request);
            inscripcion.Activo = true;
            inscripcion.FechaCreacion = DateTime.UtcNow;

            var nuevaInscripcion = await _inscripcionRepository.AddAsync(inscripcion);
            await _inscripcionRepository.SaveChangesAsync();

            _logger.LogInformation("Inscripción creada con éxito con ID: {InscripcionId}", nuevaInscripcion.Id);
            
            // 5. Devolver DTO con nombres
            var inscripcionDto = _mapper.Map<InscripcionDto>(nuevaInscripcion);
            inscripcionDto.AlumnoNombre = $"{alumno.Nombres} {alumno.ApellidoPaterno}";
            inscripcionDto.GrupoNombre = grupo.Nombre;

            return inscripcionDto;
        }

        public async Task DeleteAsync(int id)
        {
            var inscripcion = await _inscripcionRepository.GetByIdAsync(id);
            if (inscripcion == null)
            {
                throw new NotFoundException($"Inscripción con ID {id} no encontrada.");
            }

            // En lugar de borrado físico, se desactiva
            inscripcion.Activo = false;
            inscripcion.FechaActualizacion = DateTime.UtcNow;
            await _inscripcionRepository.UpdateAsync(inscripcion);
            await _inscripcionRepository.SaveChangesAsync();
            _logger.LogInformation("Inscripción con ID {Id} ha sido desactivada.", id);
        }

        public async Task<List<InscripcionDto>> GetAllAsync()
        {
            var inscripciones = await _inscripcionRepository.GetAllIncludingAsync(i => i.Alumno, i => i.Grupo);
            return _mapper.Map<List<InscripcionDto>>(inscripciones.Where(i => i.Activo));
        }
        
        public async Task<List<InscripcionDto>> GetByAlumnoIdAsync(int alumnoId)
        {
            var inscripciones = await _inscripcionRepository.GetFilteredIncludingAsync(i => i.AlumnoId == alumnoId && i.Activo, i => i.Grupo);
            if (!inscripciones.Any())
            {
                 _logger.LogWarning("No se encontraron inscripciones para el alumno con ID: {AlumnoId}", alumnoId);
            }
            return _mapper.Map<List<InscripcionDto>>(inscripciones);
        }

        public async Task<List<InscripcionDto>> GetByGrupoIdAsync(int grupoId)
        {
            var inscripciones = await _inscripcionRepository.GetFilteredIncludingAsync(i => i.GrupoId == grupoId && i.Activo, i => i.Alumno);
            if (!inscripciones.Any())
            {
                _logger.LogWarning("No se encontraron inscripciones para el grupo con ID: {GrupoId}", grupoId);
            }
            return _mapper.Map<List<InscripcionDto>>(inscripciones);
        }

        public async Task<InscripcionDto> GetByIdAsync(int id)
        {
            var inscripcion = (await _inscripcionRepository.GetFilteredIncludingAsync(i => i.Id == id, i => i.Alumno, i => i.Grupo)).FirstOrDefault();

            if (inscripcion == null || !inscripcion.Activo)
            {
                throw new NotFoundException($"Inscripción con ID {id} no encontrada o no está activa.");
            }
            
            return _mapper.Map<InscripcionDto>(inscripcion);
        }
    }
}
