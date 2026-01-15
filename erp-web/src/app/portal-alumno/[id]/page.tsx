
'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import AuthGuard from '@/components/AuthGuard';
import { alumnoService } from '@/services/alumnoService';
import { cargoService } from '@/services/cargoService';
import type { Alumno } from '@/types/alumno';
import type { Cargo } from '@/services/cargoService';
import Link from 'next/link';

export default function PortalAlumnoPage() {
  const params = useParams();
  const router = useRouter();
  const id = Number(Array.isArray(params.id) ? params.id[0] : params.id);

  const [alumno, setAlumno] = useState<Alumno | null>(null);
  const [cargos, setCargos] = useState<Cargo[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isNaN(id)) {
        setError("El ID del alumno no es válido.");
        setIsLoading(false);
        return;
    }

    const fetchData = async () => {
      try {
        setIsLoading(true);
        const [alumnoData, cargosResponse] = await Promise.all([
          alumnoService.getAlumnoById(id),
          cargoService.getCargosByAlumnoId(id),
        ]);
        
        setAlumno(alumnoData);
        setCargos(cargosResponse.data || []);
        setError(null);
      } catch (err) {
        console.error("Error al cargar los datos del portal del alumno:", err);
        setError("No se pudieron cargar los datos. Por favor, inténtelo más tarde.");
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [id]);
  
  const getStatusBadge = (estado: string) => {
    switch(estado) {
        case 'Pagado': return <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800">Pagado</span>;
        case 'Pendiente': return <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-yellow-100 text-yellow-800">Pendiente</span>;
        case 'Vencido': return <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-red-100 text-red-800">Vencido</span>;
        default: return <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-gray-100 text-gray-800">{estado}</span>;
    }
  }

  if (isLoading) return <p className="text-center mt-8">Cargando portal del alumno...</p>;
  if (error) return <p className="text-center mt-8 text-red-500 bg-red-100 p-4 rounded-md">{error}</p>;

  return (
    <AuthGuard>
      <div className="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
        <header className="mb-8">
          <div className="flex items-center justify-between">
             <div>
                <h1 className="text-3xl font-bold text-gray-900">Portal del Alumno</h1>
                {alumno && <h2 className="text-xl text-gray-600">{alumno.nombre} {alumno.apellido}</h2>}
             </div>
              <Link href="/">
                <div className="text-sm text-indigo-600 hover:text-indigo-900 cursor-pointer">
                    &larr; Volver al Dashboard
                </div>
            </Link>
          </div>
        </header>

        <main>
            <h3 className="text-xl font-semibold mb-4 text-gray-800">Estado de Cuenta</h3>
            <div className="bg-white shadow overflow-hidden sm:rounded-lg">
                <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-gray-200">
                        <thead className="bg-gray-50">
                            <tr>
                                <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Concepto</th>
                                <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Folio</th>
                                <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Fecha de Vencimiento</th>
                                <th scope="col" className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
                                <th scope="col" className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Estado</th>
                                <th scope="col" className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Acción</th>
                            </tr>
                        </thead>
                        <tbody className="bg-white divide-y divide-gray-200">
                            {cargos.length > 0 ? cargos.map((cargo) => (
                                <tr key={cargo.id}>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{cargo.concepto}</td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{cargo.folio}</td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{new Date(cargo.fechaVencimiento).toLocaleDateString()}</td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-800 text-right font-mono">${cargo.total.toFixed(2)}</td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-center">{getStatusBadge(cargo.estado)}</td>
                                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                        {cargo.estado === 'Pendiente' || cargo.estado === 'Vencido' ? (
                                            <button onClick={() => router.push(`/finanzas/pago-en-linea/${cargo.id}`)} className="text-white bg-indigo-600 hover:bg-indigo-700 px-4 py-2 rounded-md shadow-sm">
                                                Pagar en Línea
                                            </button>
                                        ) : (
                                            <span className="text-gray-400">N/A</span>
                                        )}
                                    </td>
                                </tr>
                            )) : (
                                <tr>
                                    <td colSpan={6} className="text-center py-10 px-6 text-gray-500">
                                        No hay cargos registrados para este alumno.
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </div>
            </div>
        </main>
      </div>
    </AuthGuard>
  );
}
