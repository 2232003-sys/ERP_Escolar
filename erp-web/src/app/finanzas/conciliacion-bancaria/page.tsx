
'use client';

import { useState, useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import { conciliacionService } from '@/services/conciliacionService';
import type { ConciliacionSummary } from '@/services/conciliacionService';
import { toast } from 'react-hot-toast';

export default function ConciliacionBancariaPage() {
  const [isUploading, setIsUploading] = useState(false);
  const [resultado, setResultado] = useState<ConciliacionSummary | null>(null);

  const onDrop = useCallback(async (acceptedFiles: File[]) => {
    const file = acceptedFiles[0];
    if (!file) {
      toast.error('Por favor, selecciona un archivo CSV.');
      return;
    }

    setIsUploading(true);
    setResultado(null);
    const toastId = toast.loading('Subiendo y procesando archivo...');

    try {
      const response = await conciliacionService.conciliarPagos(file);
      const data = response.data; 
      setResultado(data);
      
      if(data.pagosReconciliados > 0){
        toast.success(`${data.pagosReconciliados} pagos conciliados con éxito!`, { id: toastId });
      } else {
        toast.success('Archivo procesado. No se conciliaron nuevos pagos.', { id: toastId });
      }

    } catch (error: any) {
        const errorMessage = error.response?.data?.message || 'Error al procesar el archivo.';
        toast.error(errorMessage, { id: toastId });
        console.error(error);
    }

    setIsUploading(false);
  }, []);

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: {
      'text/csv': ['.csv'],
    },
    maxFiles: 1,
  });

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">Conciliación Bancaria por CSV</h1>
      
      <div className="bg-white p-6 rounded-lg shadow-md">
        <h2 className="text-lg font-semibold mb-2">Paso 1: Descargue su estado de cuenta</h2>
        <p className="text-sm text-gray-600 mb-4">
          Inicie sesión en el portal de su banco y descargue el estado de cuenta del período que desea conciliar. Asegúrese de guardarlo en formato <strong>.csv</strong>.
        </p>

        <h2 className="text-lg font-semibold mb-2">Paso 2: Suba el archivo aquí</h2>
        <div
          {...getRootProps()}
          className={`border-2 border-dashed rounded-lg p-12 text-center cursor-pointer transition-colors 
            ${isDragActive ? 'border-blue-500 bg-blue-50' : 'border-gray-300 bg-gray-50 hover:border-gray-400'}`}
        >
          <input {...getInputProps()} />
          {
            isUploading ?
            <p className="text-gray-700">Procesando...</p> :
            isDragActive ?
              <p className="text-blue-700">¡Suelta el archivo aquí!</p> :
              <p className="text-gray-500">Arrastra y suelta el archivo .csv aquí, o haz clic para seleccionarlo.</p>
          }
        </div>
      </div>

      {resultado && (
        <div className="mt-6 bg-white p-6 rounded-lg shadow-md">
          <h2 className="text-xl font-bold mb-4">Resultados de la Conciliación</h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 text-center">
            <div className="p-4 bg-gray-100 rounded-lg">
              <p className="text-3xl font-bold">{resultado.totalTransacciones}</p>
              <p className="text-gray-600">Transacciones en el archivo</p>
            </div>
            <div className="p-4 bg-green-100 rounded-lg">
              <p className="text-3xl font-bold text-green-800">{resultado.pagosReconciliados}</p>
              <p className="text-green-700">Pagos conciliados exitosamente</p>
            </div>
            <div className={`p-4 ${resultado.errores > 0 ? 'bg-red-100' : 'bg-gray-100'} rounded-lg`}>
              <p className={`text-3xl font-bold ${resultado.errores > 0 ? 'text-red-800' : ''}`}>{resultado.errores}</p>
              <p className={`${resultado.errores > 0 ? 'text-red-700' : 'text-gray-600'}`}>Necesitan atención manual</p>
            </div>
          </div>

          {resultado.detallesErrores && resultado.detallesErrores.length > 0 && (
            <div className="mt-6">
              <h3 className="font-semibold">Detalles de errores y advertencias:</h3>
              <ul className="list-disc list-inside mt-2 bg-gray-50 p-4 rounded-md text-sm text-gray-700">
                {resultado.detallesErrores.map((error, index) => (
                  <li key={index} className="mt-1">{error}</li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
