
'use client';

import AuthGuard from '@/components/AuthGuard';
import { authService } from '@/services/authService';
import Link from 'next/link';

export default function Home() {
  const handleLogout = () => {
    authService.logout();
  };

  return (
    <AuthGuard>
      <div className="min-h-screen bg-gray-50">
        <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
          <div className="px-4 py-6 sm:px-0">
            <div className="flex justify-between items-center mb-8">
              <h1 className="text-3xl font-bold text-gray-900">
                Dashboard - ERP Escolar
              </h1>
              <button
                onClick={handleLogout}
                className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-md text-sm font-medium"
              >
                Cerrar Sesión
              </button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              {/* Control Escolar */}
              <Link href="/control-escolar/alumnos">
                <div className="bg-white overflow-hidden shadow rounded-lg cursor-pointer hover:bg-gray-100 transition-colors duration-200">
                  <div className="p-5">
                    <div className="flex items-center">
                      <div className="flex-shrink-0">
                        <div className="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                          <span className="text-white text-sm font-medium">CE</span>
                        </div>
                      </div>
                      <div className="ml-5 w-0 flex-1">
                        <dl>
                          <dt className="text-sm font-medium text-gray-500 truncate">
                            Control Escolar
                          </dt>
                          <dd className="text-lg font-medium text-gray-900">
                            Gestión de Alumnos
                          </dd>
                        </dl>
                      </div>
                    </div>
                  </div>
                </div>
              </Link>

              {/* Generar Cargos */}
              <Link href="/finanzas/generar-cargos">
                <div className="bg-white overflow-hidden shadow rounded-lg cursor-pointer hover:bg-gray-100 transition-colors duration-200">
                  <div className="p-5">
                    <div className="flex items-center">
                      <div className="flex-shrink-0">
                        <div className="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                          <span className="text-white text-xl font-bold">$</span>
                        </div>
                      </div>
                      <div className="ml-5 w-0 flex-1">
                        <dl>
                          <dt className="text-sm font-medium text-gray-500 truncate">
                            Finanzas
                          </dt>
                          <dd className="text-lg font-medium text-gray-900">
                            Generar Cargos
                          </dd>
                        </dl>
                      </div>
                    </div>
                  </div>
                </div>
              </Link>

              {/* Conciliación Bancaria */}
              <Link href="/finanzas/conciliacion-bancaria">
                 <div className="bg-white overflow-hidden shadow rounded-lg cursor-pointer hover:bg-gray-100 transition-colors duration-200">
                  <div className="p-5">
                    <div className="flex items-center">
                      <div className="flex-shrink-0">
                        <div className="w-8 h-8 bg-yellow-500 rounded-md flex items-center justify-center">
                          <span className="text-white text-sm font-medium">CB</span>
                        </div>
                      </div>
                      <div className="ml-5 w-0 flex-1">
                        <dl>
                          <dt className="text-sm font-medium text-gray-500 truncate">
                            Finanzas
                          </dt>
                          <dd className="text-lg font-medium text-gray-900">
                            Conciliación Bancaria
                          </dd>
                        </dl>
                      </div>
                    </div>
                  </div>
                </div>
              </Link>

              {/* Gestión Fiscal (CFDI) */}
              <Link href="/fiscal/gestion-cfdi">
                <div className="bg-white overflow-hidden shadow rounded-lg cursor-pointer hover:bg-gray-100 transition-colors duration-200">
                  <div className="p-5">
                    <div className="flex items-center">
                      <div className="flex-shrink-0">
                        <div className="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                           <span className="text-white text-sm font-medium">CFDI</span>
                        </div>
                      </div>
                      <div className="ml-5 w-0 flex-1">
                        <dl>
                          <dt className="text-sm font-medium text-gray-500 truncate">
                            Fiscal
                          </dt>
                          <dd className="text-lg font-medium text-gray-900">
                            Gestión de Facturas
                          </dd>
                        </dl>
                      </div>
                    </div>
                  </div>
                </div>
              </Link>
            </div>

            {/* Espacio para futuros Widgets o Estadísticas */}
            <div className="mt-8">
               {/* Aquí podemos re-introducir widgets de estadísticas si es necesario */}
            </div>

          </div>
        </div>
      </div>
    </AuthGuard>
  );
}
