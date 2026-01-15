
'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import AuthGuard from '@/components/AuthGuard';
import { alumnoService } from '@/services/alumnoService';
import type { CreateAlumnoDto } from '@/types/alumno';

export default function NuevoAlumnoPage() {
  const router = useRouter();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [formData, setFormData] = useState<Omit<CreateAlumnoDto, 'schoolId' | 'tutorId'>>({
    nombre: '',
    apellido: '',
    email: '',
    curp: '',
    fechaNacimiento: '',
    sexo: 'M',
    direccion: '',
    telefonoContacto: ''
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      // Hardcodeamos schoolId y tutorId por ahora. En una app real, vendrían del contexto o props.
      const alumnoData: CreateAlumnoDto = {
        ...formData,
        schoolId: 1,
      };

      await alumnoService.createAlumno(alumnoData);
      
      // Idealmente, mostraríamos un toast de éxito aquí.
      alert('¡Alumno creado con éxito!');
      router.push('/control-escolar/alumnos');
      // Podríamos incluso forzar una recarga de los datos en la página anterior.

    } catch (err) {
      setError('No se pudo crear el alumno. Revisa los datos e inténtalo de nuevo.');
      console.error(err);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <AuthGuard>
      <div className="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
        <header className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Agregar Nuevo Alumno</h1>
          <button onClick={() => router.back()} className="text-sm text-indigo-600 hover:text-indigo-900 mt-2">
            &larr; Volver a la lista
          </button>
        </header>

        <form onSubmit={handleSubmit} className="bg-white shadow-md rounded-lg p-8">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Nombre */}
            <div>
              <label htmlFor="nombre" className="block text-sm font-medium text-gray-700">Nombre</label>
              <input type="text" name="nombre" id="nombre" required className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" onChange={handleChange} value={formData.nombre} />
            </div>

            {/* Apellido */}
            <div>
              <label htmlFor="apellido" className="block text-sm font-medium text-gray-700">Apellido</label>
              <input type="text" name="apellido" id="apellido" required className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" onChange={handleChange} value={formData.apellido} />
            </div>

            {/* Email */}
            <div className="md:col-span-2">
              <label htmlFor="email" className="block text-sm font-medium text-gray-700">Email</label>
              <input type="email" name="email" id="email" required className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" onChange={handleChange} value={formData.email} />
            </div>

            {/* CURP */}
            <div>
              <label htmlFor="curp" className="block text-sm font-medium text-gray-700">CURP</label>
              <input type="text" name="curp" id="curp" required pattern="[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[A-Z0-9]{2}" title="CURP no válido" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" onChange={handleChange} value={formData.curp} />
            </div>

            {/* Fecha de Nacimiento */}
            <div>
              <label htmlFor="fechaNacimiento" className="block text-sm font-medium text-gray-700">Fecha de Nacimiento</label>
              <input type="date" name="fechaNacimiento" id="fechaNacimiento" required className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" onChange={handleChange} value={formData.fechaNacimiento} />
            </div>
            
            {/* Sexo */}
            <div>
              <label htmlFor="sexo" className="block text-sm font-medium text-gray-700">Sexo</label>
              <select id="sexo" name="sexo" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" onChange={handleChange} value={formData.sexo}>
                <option value="M">Masculino</option>
                <option value="F">Femenino</option>
              </select>
            </div>

            {/* Dirección */}
            <div className="md:col-span-2">
              <label htmlFor="direccion" className="block text-sm font-medium text-gray-700">Dirección (Opcional)</label>
              <input type="text" name="direccion" id="direccion" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" onChange={handleChange} value={formData.direccion} />
            </div>

            {/* Teléfono de Contacto */}
            <div className="md:col-span-2">
              <label htmlFor="telefonoContacto" className="block text-sm font-medium text-gray-700">Teléfono de Contacto (Opcional)</label>
              <input type="tel" name="telefonoContacto" id="telefonoContacto" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" onChange={handleChange} value={formData.telefonoContacto} />
            </div>
          </div>

          {error && <p className="mt-4 text-sm text-red-600 font-medium">Error: {error}</p>}

          <div className="mt-8 flex justify-end">
            <button type="button" onClick={() => router.back()} className="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
              Cancelar
            </button>
            <button type="submit" disabled={isSubmitting} className="ml-3 inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50">
              {isSubmitting ? 'Guardando...' : 'Guardar Alumno'}
            </button>
          </div>
        </form>
      </div>
    </AuthGuard>
  );
}
