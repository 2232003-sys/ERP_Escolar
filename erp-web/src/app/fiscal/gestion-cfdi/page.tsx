
'use client';

import { useState, useEffect, useCallback } from 'react';
import { cfdiService } from '@/services/cfdiService';
// La importación de tipos ahora apunta a la fuente unificada
import type { CFDI } from '@/lib/api/types'; 
import { toast } from 'react-hot-toast';

export default function GestionCFDIPage() {
  const [cfdiList, setCfdiList] = useState<CFDI[]>([]);
  // selectedCfdi podría necesitar un tipo más detallado que no tenemos, se elimina por ahora
  // const [selectedCfdi, setSelectedCfdi] = useState<CFDIFull | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  
  const fetchCFDIs = useCallback(async () => {
    setIsLoading(true);
    try {
      const response = await cfdiService.getAllCfdi();
      // La capa de servicio devuelve directamente la data de la respuesta de apiClient
      // La cual ya es del tipo ApiResponse, por lo que la estructura es { success, data, message }
      if (response.success) {
        setCfdiList(response.data);
      } else {
        toast.error(response.message || 'Error al cargar la lista de CFDI.');
      }
    } catch (error) {
      toast.error('Error fatal al conectar con el servicio de CFDI.');
    }
    setIsLoading(false);
  }, []);

  useEffect(() => {
    fetchCFDIs();
  }, [fetchCFDIs]);

  const handleViewDetails = (id: number) => {
    toast.error('La carga de detalles no está implementada.');
  };

  const handleTimbrar = (id: number) => {
    toast.error('La funcionalidad de timbrar no está implementada.');
  };

  const handleCancelar = (id: number) => {
      toast.error('La funcionalidad de cancelar no está implementada.');
  };

  const getStatusChip = (status: string) => {
    const baseClasses = 'px-2 py-1 text-xs font-semibold rounded-full';
    switch (status) {
      case 'Timbrada': return `${baseClasses} bg-green-200 text-green-800`;
      case 'Borrador': return `${baseClasses} bg-yellow-200 text-yellow-800`;
      case 'Cancelada': return `${baseClasses} bg-red-200 text-red-800`;
      default: return `${baseClasses} bg-gray-200 text-gray-800`;
    }
  };

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">Gestión de Facturas (CFDI)</h1>
      
      {isLoading ? (
        <p>Cargando facturas...</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full bg-white">
            <thead className="bg-gray-50">
              <tr>
                <th className="py-2 px-4 border-b">Folio</th>
                <th className="py-2 px-4 border-b">Receptor</th>
                <th className="py-2 px-4 border-b">Total</th>
                <th className="py-2 px-4 border-b">Estado</th>
                <th className="py-2 px-4 border-b">UUID</th>
                <th className="py-2 px-4 border-b">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {cfdiList.map((cfdi) => (
                <tr key={cfdi.id}>
                  <td className="py-2 px-4 border-b">{cfdi.serie}-{cfdi.folio}</td>
                  {/* El tipo CFDI unificado SÍ tiene nombreReceptor */}
                  <td className="py-2 px-4 border-b">{cfdi.nombreReceptor}</td>
                  <td className="py-2 px-4 border-b">${cfdi.total.toFixed(2)}</td>
                  <td className="py-2 px-4 border-b">
                    {/* El tipo CFDI unificado usa 'estado', no 'status' */}
                    <span className={getStatusChip(cfdi.estado)}>{cfdi.estado}</span>
                  </td>
                  <td className="py-2 px-4 border-b text-xs">{cfdi.uuid || 'N/A'}</td>
                  <td className="py-2 px-4 border-b space-x-2">
                    <button onClick={() => handleViewDetails(cfdi.id)} className="text-blue-500 hover:underline">Detalles</button>
                    {cfdi.estado === 'Borrador' && 
                        <button onClick={() => handleTimbrar(cfdi.id)} className="text-green-500 hover:underline">Timbrar</button>}
                    {cfdi.estado === 'Timbrada' && 
                        <button onClick={() => handleCancelar(cfdi.id)} className="text-red-500 hover:underline">Cancelar</button>}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Modal deshabilitado temporalmente */}
    </div>
  );
}
