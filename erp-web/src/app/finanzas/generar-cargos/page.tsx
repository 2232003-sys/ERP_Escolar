'use client';

import { useState } from "react";
import { finanzasService, GeneracionMasivaDto } from "@/services/finanzasService";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { toast } from 'react-hot-toast';

export default function GenerarCargosPage() {
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [formData, setFormData] = useState<Omit<GeneracionMasivaDto, 'idsAlumnos'> >({
    concepto: '',
    monto: 0,
    fechaVencimiento: '',
  });

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'number' ? parseFloat(value) || 0 : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    const toastId = toast.loading('Iniciando generación de cargos...');

    const requestData: GeneracionMasivaDto = {
        ...formData,
        fechaVencimiento: new Date(formData.fechaVencimiento).toISOString(),
    };

    try {
      const response = await finanzasService.generarCargosMasivos(requestData);
      toast.success(`Proceso finalizado. Se generaron ${response.data.cargosGenerados} cargos.`, { id: toastId });
      // Reset form
      setFormData({ concepto: '', monto: 0, fechaVencimiento: '' });
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || "Ocurrió un error desconocido.";
      toast.error(errorMessage, { id: toastId });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="grid place-items-center h-full p-4">
      <Card className="w-full max-w-lg">
        <CardHeader>
          <CardTitle>Generación Masiva de Cargos</CardTitle>
          <CardDescription>
            Crea un nuevo cargo y aplícalo a todos los alumnos activos de forma simultánea. Ideal para colegiaturas, cuotas de materiales o inscripciones.
          </CardDescription>
        </CardHeader>
        <form onSubmit={handleSubmit}>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="concepto">Concepto del Cargo</Label>
              <Input
                id="concepto"
                name="concepto"
                value={formData.concepto}
                onChange={handleInputChange}
                placeholder="Ej: Colegiatura Enero 2024"
                required
                disabled={isLoading}
              />
            </div>
             <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div className="space-y-2">
                    <Label htmlFor="monto">Monto (MXN)</Label>
                    <Input
                    id="monto"
                    name="monto"
                    type="number"
                    value={formData.monto}
                    onChange={handleInputChange}
                    placeholder="Ej: 3500.00"
                    required
                    min="0"
                    step="0.01"
                    disabled={isLoading}
                    />
                </div>
                <div className="space-y-2">
                    <Label htmlFor="fechaVencimiento">Fecha de Vencimiento</Label>
                    <Input
                    id="fechaVencimiento"
                    name="fechaVencimiento"
                    type="date"
                    value={formData.fechaVencimiento}
                    onChange={handleInputChange}
                    required
                    disabled={isLoading}
                    />
                </div>
            </div>
          </CardContent>
          <CardFooter className="flex flex-col items-start">
            <Button type="submit" disabled={isLoading || !formData.concepto || formData.monto <= 0 || !formData.fechaVencimiento} className="w-full sm:w-auto">
              {isLoading ? "Procesando..." : "Iniciar Generación Masiva"}
            </Button>
          </CardFooter>
        </form>
      </Card>
    </div>
  );
}
