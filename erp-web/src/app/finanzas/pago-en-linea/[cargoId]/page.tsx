
'use client';

import { useEffect, useState } from 'react';
import { loadStripe } from '@stripe/stripe-js';
import { Elements } from '@stripe/react-stripe-js';
import { procesadorPagoService } from '@/services/procesadorPagoService';
import CheckoutForm from './CheckoutForm'; // Crearemos este componente a continuación
import { useParams } from 'next/navigation';

// Carga la clave pública de Stripe (es seguro exponerla)
const stripePromise = loadStripe(process.env.NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY || '');

export default function PagoEnLineaPage() {
  const [clientSecret, setClientSecret] = useState('');
  const [error, setError] = useState<string | null>(null);
  const params = useParams();
  const cargoId = params.cargoId as string;

  useEffect(() => {
    if (!cargoId) {
      setError('No se ha especificado un cargo para pagar.');
      return;
    }

    const getPaymentIntent = async () => {
      try {
        const response = await procesadorPagoService.createPaymentIntent(cargoId);
        if(response.data.clientSecret){
            setClientSecret(response.data.clientSecret);
        } else {
            setError(response.message || 'No se pudo iniciar el proceso de pago.');
        }
      } catch (err: any) {
        console.error("Error al obtener el payment intent", err);
        setError(err.response?.data?.message || 'Error al comunicarse con el servidor de pagos.');
      }
    };

    getPaymentIntent();
  }, [cargoId]);

  const appearance = {
    theme: 'stripe' as const,
    variables: {
        colorPrimary: '#0570de',
        colorBackground: '#ffffff',
        colorText: '#30313d',
        colorDanger: '#df1b41',
        fontFamily: 'Ideal Sans, system-ui, sans-serif',
        spacingUnit: '2px',
        borderRadius: '4px',
      },
  };

  const options = {
    clientSecret,
    appearance,
  };

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center">
        <div className="w-full max-w-md p-8 space-y-6 bg-white rounded-lg shadow-md">
            <h1 className="text-2xl font-bold text-center text-gray-800">Pago Seguro en Línea</h1>
            
            {error && (
                <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 rounded-md">
                    <h2 className="font-bold">Error</h2>
                    <p>{error}</p>
                </div>
            )}

            {clientSecret && (
                <Elements options={options} stripe={stripePromise}>
                    <CheckoutForm />
                </Elements>
            )}

            {!clientSecret && !error && (
                <div className="text-center">
                    <p className="text-gray-600">Estableciendo conexión segura con el portal de pagos...</p>
                     {/* Aquí podría ir un spinner o loader */}
                </div>
            )}
        </div>
    </div>
  );
}
