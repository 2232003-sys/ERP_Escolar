
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

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
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
                            Alumnos, Grupos, Inscripciones
                          </dd>
                        </dl>
                      </div>
                    </div>
                  </div>
                </div>
              </Link>

              {/* Finanzas */}
              <div className="bg-white overflow-hidden shadow rounded-lg">
                <div className="p-5">
                  <div className="flex items-center">
                    <div className="flex-shrink-0">
                      <div className="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                        <span className="text-white text-sm font-medium">$</span>
                      </div>
                    </div>
                    <div className="ml-5 w-0 flex-1">
                      <dl>
                        <dt className="text-sm font-medium text-gray-500 truncate">
                          Finanzas
                        </dt>
                        <dd className="text-lg font-medium text-gray-900">
                          Cargos, Pagos, Reportes
                        </dd>
                      </dl>
                    </div>
                  </div>
                </div>
              </div>

              {/* Fiscal */}
              <div className="bg-white overflow-hidden shadow rounded-lg">
                <div className="p-5">
                  <div className="flex items-center">
                    <div className="flex-shrink-0">
                      <div className="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                        <span className="text-white text-sm font-medium">F</span>
                      </div>
                    </div>
                    <div className="ml-5 w-0 flex-1">
                      <dl>
                        <dt className="text-sm font-medium text-gray-500 truncate">
                          Fiscal
                        </dt>
                        <dd className="text-lg font-medium text-gray-900">
                          CFDI, Facturación
                        </dd>
                      </dl>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Estadísticas */}
            <div className="mt-8">
              <div className="bg-white shadow rounded-lg">
                <div className="px-4 py-5 sm:p-6">
                  <h3 className="text-lg leading-6 font-medium text-gray-900 mb-4">
                    Estadísticas Generales
                  </h3>
                  <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                    <div className="text-center">
                      <div className="text-3xl font-bold text-blue-600">1,234</div>
                      <div className="text-sm text-gray-500">Total Alumnos</div>
                    </div>
                    <div className="text-center">
                      <div className="text-3xl font-bold text-green-600">89</div>
                      <div className="text-sm text-gray-500">Grupos Activos</div>
                    </div>
                    <div className="text-center">
                      <div className="text-3xl font-bold text-purple-600">156</div>
                      <div className="text-sm text-gray-500">Inscripciones</div>
                    </div>
                    <div className="text-center">
                      <div className="text-3xl font-bold text-orange-600">98.5%</div>
                      <div className="text-sm text-gray-500">Asistencia Promedio</div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Estado del Sistema */}
            <div className="mt-8">
              <div className="bg-white shadow rounded-lg">
                <div className="px-4 py-5 sm:p-6">
                  <h3 className="text-lg leading-6 font-medium text-gray-900 mb-4">
                    Estado del Sistema
                  </h3>
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <div className="text-center">
                      <div className="text-2xl font-bold text-green-600">✅</div>
                      <div className="text-sm text-gray-500">API Backend</div>
                      <div className="text-xs text-gray-400">http://localhost:5235</div>
                    </div>
                    <div className="text-center">
                      <div className="text-2xl font-bold text-green-600">✅</div>
                      <div className="text-sm text-gray-500">Base de Datos</div>
                      <div className="text-xs text-gray-400">PostgreSQL</div>
                    </div>
                    <div className="text-center">
                      <div className="text-2xl font-bold text-green-600">✅</div>
                      <div className="text-sm text-gray-500">Autenticación</div>
                      <div className="text-xs text-gray-400">JWT + RBAC</div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </AuthGuard>
  );
}
