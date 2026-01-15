
'use client';

import { useEffect, useState } from 'react';
import { useRouter, useParams } from 'next/navigation';
import AuthGuard from '@/components/AuthGuard';
import { alumnoService } from '@/services/alumnoService';
import type { Alumno, UpdateAlumnoDto } from '@/types/alumno';

export default function EditarAlumnoPage() {
  const router = useRouter();
  const params = useParams();
  const id = Array.isArray(params.id) ? params.id[0] : params.id;

  const [formData, setFormData] = useState<UpdateAlumnoDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (id) {
      const fetchAlumno = async () => {
        try {
          setIsLoading(true);
          const fetchedAlumno = await alumnoService.getAlumnoById(Number(id));
          // Formateamos la fecha para el input type="date"
          const formattedDate = new Date(fetchedAlumno.fechaNacimiento).toISOString().split('T')[0];
          setFormData({
            nombre: fetchedAlumno.nombre,
            apellido: fetchedAlumno.apellido,
            email: fetchedAlumno.email,
            fechaNacimiento: formattedDate,
            sexo: fetchedAlumno.sexo,
          });
          setError(null);
        } catch (err) {
          setError('No se pudieron cargar los datos del alumno. Inténtalo de nuevo más tarde.');
          console.error(err);
        } finally {
          setIsLoading(false);
        }
      };

      fetchAlumno();
    }
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    if (formData) {
        setFormData(prev => prev ? { ...prev, [name]: value } : null);
    }
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!formData || !id) return;

    setIsSubmitting(true);
    setError(null);

    try {
      await alumnoService.updateAlumno(Number(id), formData);
      alert('¡Alumno actualizado con éxito!');
      router.push('/control-escolar/alumnos');
    } catch (err) {
      setError('No se pudo actualizar el alumno. Revisa los datos e inténtalo de nuevo.');
      console.error(err);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) return <p className="text-center mt-8">Cargando datos del alumno...</p>;
  if (error) return <p className="text-center mt-8 text-red-500 bg-red-100 p-4 rounded-md">{error}</p>;
  if (!formData) return <p className="text-center mt-8">No se encontraron datos para este alumno.</p>;

  return (
    <AuthGuard>
      <div className="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
        <header className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Editar Alumno</h1>
          <button onClick={() => router.back()} className="text-sm text-indigo-600 hover:text-indigo-900 mt-2">
            &larr; Volver a la lista
          </button>
        </header>

        <form onSubmit={handleSubmit} className="bg-white shadow-md rounded-lg p-8">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Nombre */}
            <div>
              <label htmlFor="nombre" className="block text-sm font-medium text-gray-700">Nombre</label>
              <input type="text" name="nombre" id="nombre" required value={formData.nombre} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" />
            </div>

            {/* Apellido */}
            <div>
              <label htmlFor="apellido" className="block text-sm font-medium text-gray-700">Apellido</label>
              <input type="text" name="apellido" id="apellido" required value={formData.apellido} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" />
            </div>

            {/* Email */}
            <div className="md:col-span-2">
              <label htmlFor="email" className="block text-sm font-medium text-gray-700">Email</label>
              <input type="email" name="email" id="email" required value={formData.email} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" />
            </div>

            {/* Fecha de Nacimiento */}
            <div>
              <label htmlFor="fechaNacimiento" className="block text-sm font-medium text-gray-700">Fecha de Nacimiento</label>
              <input type="date" name="fechaNacimiento" id="fechaNacimiento" required value={formData.fechaNacimiento} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" />
            </div>

            {/* Sexo */}
            <div>
              <label htmlFor="sexo" className="block text-sm font-medium text-gray-700">Sexo</label>
              <select id="sexo" name="sexo" value={formData.sexo} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm">
                <option value="M">Masculino</option>
                <option value="F">Femenino</option>
              </select>
            </div>
          </div>

          <div className="mt-8 flex justify-end">
            <button type="button" onClick={() => router.back()} className="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50">
              Cancelar
            </button>
            <button type="submit" disabled={isSubmitting} className="ml-3 inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 disabled:opacity-50">
              {isSubmitting ? 'Guardando cambios...' : 'Guardar Cambios'}
            </button>
          </div>
        </form>
      </div>
    </AuthGuard>
  );
}
