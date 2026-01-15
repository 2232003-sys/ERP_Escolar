
'use client';

import { useState, useEffect } from 'react';
import { alumnoService } from '@/services/alumnoService';
import { finanzasService } from '@/services/finanzasService';
import type { Alumno } from '@/types/alumno';
import type { EstadoCuenta, Cargo, RegistrarPagoDto, MetodoPago } from '@/types/finanzas';
import { toast } from 'react-hot-toast';

export default function RegistrarPagoPage() {
  const [alumnos, setAlumnos] = useState<Alumno[]>([]);
  const [selectedAlumnoId, setSelectedAlumnoId] = useState<number | null>(null);
  const [estadoCuenta, setEstadoCuenta] = useState<EstadoCuenta | null>(null);
  const [selectedCargo, setSelectedCargo] = useState<Cargo | null>(null);
  const [monto, setMonto] = useState<number>(0);
  const [metodo, setMetodo] = useState<MetodoPago>('Efectivo');
  const [fechaPago, setFechaPago] = useState<string>(
    new Date().toISOString().split('T')[0]
  );

  useEffect(() => {
    const fetchAlumnos = async () => {
      try {
        const data = await alumnoService.getAllAlumnos();
        setAlumnos(data);
      } catch (error) {
        toast.error('No se pudieron cargar los alumnos.');
      }
    };
    fetchAlumnos();
  }, []);

  useEffect(() => {
    if (selectedAlumnoId) {
      const fetchEstadoCuenta = async () => {
        try {
          const response = await alumnoService.getEstadoCuenta(selectedAlumnoId);
          setEstadoCuenta(response.data);
          setSelectedCargo(null);
        } catch (error) {
          toast.error('Error al cargar el estado de cuenta.');
          setEstadoCuenta(null);
        }
      };
      fetchEstadoCuenta();
    }
  }, [selectedAlumnoId]);

  const handleRegistrarPago = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedAlumnoId || !selectedCargo || monto <= 0) {
      toast.error('Por favor, complete todos los campos.');
      return;
    }

    const requestData: RegistrarPagoDto = {
      alumnoId: selectedAlumnoId,
      cargoId: selectedCargo.id.toString(),
      monto,
      metodoPago: metodo,
      fechaPago,
      referenciaExterna: '',
      observacion: `Pago para el cargo ${selectedCargo.folio}`
    };

    try {
        await finanzasService.registrarPagoManual(requestData);
        toast.success('¡Pago registrado exitosamente!');
        // Resetear formulario y recargar estado de cuenta
        setSelectedCargo(null);
        setMonto(0);
        if(selectedAlumnoId) {
            const response = await alumnoService.getEstadoCuenta(selectedAlumnoId);
            setEstadoCuenta(response.data);
        }
    } catch (error) {
        toast.error('Hubo un error al registrar el pago.');
    }
  };

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">Registrar Nuevo Pago</h1>

      {/* Selector de Alumno */}
      <div className="mb-4">
        <label htmlFor="alumno" className="block text-sm font-medium text-gray-700">
          Seleccione un Alumno
        </label>
        <select
          id="alumno"
          className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
          onChange={(e) => setSelectedAlumnoId(Number(e.target.value))}
        >
          <option>-- Seleccionar --</option>
          {alumnos.map((a) => (
            <option key={a.id} value={a.id}>
              {a.nombre} {a.apellido}
            </option>
          ))}
        </select>
      </div>

      {/* Estado de Cuenta y Formulario de Pago */}
      {estadoCuenta && (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {/* Lista de Cargos Pendientes */}
          <div>
            <h2 className="text-xl font-semibold mb-2">Cargos Pendientes</h2>
            <div className="space-y-2">
              {estadoCuenta.cargos
                .filter((c) => c.estado === 'Pendiente' || c.estado === 'Parcial')
                .map((cargo) => (
                  <div
                    key={cargo.id}
                    className={`p-3 rounded-md cursor-pointer ${selectedCargo?.id === cargo.id ? 'bg-blue-200 border-blue-500' : 'bg-gray-100'}`}
                    onClick={() => {
                        setSelectedCargo(cargo)
                        setMonto(cargo.total - cargo.montoRecibido) // Sugerir monto restante
                    }}
                  >
                    <p className="font-bold">{cargo.concepto}</p>
                    <p>Total: ${cargo.total.toFixed(2)}</p>
                    <p>Pendiente: ${(cargo.total - cargo.montoRecibido).toFixed(2)}</p>
                    <p>Vencimiento: {cargo.fechaVencimiento?.split('T')[0]}</p>
                  </div>
                ))}
            </div>
          </div>

          {/* Formulario de Pago */}
          {selectedCargo && (
            <form onSubmit={handleRegistrarPago} className="p-4 border rounded-md">
                <h3 className="text-lg font-semibold">Detalles del Pago</h3>
                <p>Pagando cargo: <strong>{selectedCargo.concepto}</strong></p>
                
                {/* Monto */}
                <div className="mt-4">
                    <label htmlFor="monto">Monto a Pagar</label>
                    <input type="number" id="monto" value={monto} onChange={e => setMonto(Number(e.target.value))} required className="w-full p-2 border rounded"/>
                </div>

                {/* Método de Pago */}
                 <div className="mt-4">
                    <label htmlFor="metodo">Método de Pago</label>
                    <select id="metodo" value={metodo} onChange={e => setMetodo(e.target.value as MetodoPago)} className="w-full p-2 border rounded">
                        <option value="Efectivo">Efectivo</option>
                        <option value="TarjetaCredito">Tarjeta de Crédito</option>
                        <option value="TarjetaDebito">Tarjeta de Débito</option>
                        <option value="Transferencia">Transferencia</option>
                    </select>
                </div>

                {/* Fecha de Pago */}
                 <div className="mt-4">
                    <label htmlFor="fecha">Fecha del Pago</label>
                    <input type="date" id="fecha" value={fechaPago} onChange={e => setFechaPago(e.target.value)} required className="w-full p-2 border rounded"/>
                </div>

                <button type="submit" className="mt-6 w-full bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded">
                    Registrar Pago
                </button>
            </form>
          )}
        </div>
      )}
    </div>
  );
}
