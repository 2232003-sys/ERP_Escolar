
'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import AuthGuard from '@/components/AuthGuard';
import { alumnoService } from '@/services/alumnoService';
import type { Alumno } from '@/types/alumno';

export default function AlumnosPage() {
  const [alumnos, setAlumnos] = useState<Alumno[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchAlumnos = async () => {
    try {
      setIsLoading(true);
      const data = await alumnoService.getAllAlumnos();
      setAlumnos(data);
      setError(null);
    } catch (err) {
      setError('No se pudieron cargar los alumnos. Inténtalo de nuevo más tarde.');
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchAlumnos();
  }, []);

  const handleDelete = async (id: number) => {
    if (window.confirm('¿Estás seguro de que deseas eliminar este alumno? Esta acción no se puede deshacer.')) {
      try {
        await alumnoService.deleteAlumno(id);
        // Actualiza el estado para reflejar la eliminación en la UI
        setAlumnos(prevAlumnos => prevAlumnos.filter(alumno => alumno.id !== id));
        // Opcional: mostrar un toast de éxito
        alert('Alumno eliminado con éxito.');
      } catch (err) {
        setError('No se pudo eliminar el alumno. Inténtalo de nuevo.');
        console.error(err);
      }
    }
  };

  return (
    <AuthGuard>
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Gestión de Alumnos</h1>
            <p className="mt-1 text-sm text-gray-500">
              Aquí puedes ver, agregar, editar y eliminar alumnos del sistema.
            </p>
          </div>
          <div>
            <Link href="/control-escolar/alumnos/nuevo">
              <button
                type="button"
                className="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                + Agregar Alumno
              </button>
            </Link>
          </div>
        </div>

        <main>
          {isLoading && <p className="text-center text-gray-500">Cargando alumnos...</p>}
          {error && <p className="text-center text-red-500 bg-red-100 p-3 rounded-md">{error}</p>}
          {!isLoading && !error && alumnos.length > 0 && (
            <div className="overflow-x-auto shadow-md sm:rounded-lg">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Nombre Completo
                    </th>
                    <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Matrícula
                    </th>
                    <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Email
                    </th>
                    <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Estado
                    </th>
                    <th scope="col" className="relative px-6 py-3">
                      <span className="sr-only">Acciones</span>
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {alumnos.map((alumno) => (
                    <tr key={alumno.id}>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-medium text-gray-900">
                          {alumno.nombre} {alumno.apellido}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm text-gray-500">{alumno.matricula || 'N/A'}</div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm text-gray-500">{alumno.email}</div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {
                          alumno.activo ? (
                            <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800">
                              Activo
                            </span>
                          ) : (
                            <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-red-100 text-red-800">
                              Inactivo
                            </span>
                          )
                        }
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                        <Link href={`/control-escolar/alumnos/editar/${alumno.id}`}>
                           <button className="text-indigo-600 hover:text-indigo-900">Editar</button>
                        </Link>
                        <button onClick={() => handleDelete(alumno.id)} className="ml-4 text-red-600 hover:text-red-900">
                          Eliminar
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
           {!isLoading && !error && alumnos.length === 0 && (
            <div className="text-center py-12">
              <h2 className="text-lg font-medium text-gray-900">No se encontraron alumnos</h2>
              <p className="mt-1 text-sm text-gray-500">Parece que aún no hay alumnos registrados en el sistema.</p>
               <div className="mt-6">
                <Link href="/control-escolar/alumnos/nuevo">
                  <button
                    type="button"
                    className="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                  >
                    + Agregar Alumno
                  </button>
                </Link>
              </div>
            </div>
          )}
        </main>
      </div>
    </AuthGuard>
  );
}
