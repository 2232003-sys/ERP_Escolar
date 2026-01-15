
'use client';

import { useEffect, useState, Suspense } from 'react';
import { useSearchParams, useRouter } from 'next/navigation';
import Link from 'next/link';

function StatusDisplay() {
    const searchParams = useSearchParams();
    const router = useRouter();
    
    // Asumimos que la clave pública está disponible como variable de entorno
    const stripePromise = import('@stripe/stripe-js').then(stripe => stripe.loadStripe(process.env.NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY || ''));
    
    const [message, setMessage] = useState<string | null>('Verificando estado del pago...');
    const [status, setStatus] = useState<'success' | 'processing' | 'error' | 'loading'>('loading');

    useEffect(() => {
        const clientSecret = searchParams.get('payment_intent_client_secret');
        
        if (!clientSecret) {
            setStatus('error');
            setMessage('No se encontró la información del pago para verificar.');
            return;
        }

        stripePromise.then(stripe => {
            if (!stripe) {
                 setStatus('error');
                 setMessage('Error al cargar el componente de pago.');
                 return;
            }

            stripe.retrievePaymentIntent(clientSecret).then(({ paymentIntent }) => {
                switch (paymentIntent?.status) {
                    case 'succeeded':
                        setStatus('success');
                        setMessage('¡Pago completado! Su pago ha sido procesado exitosamente. Recibirá una confirmación por correo electrónico en breve.');
                        break;
                    case 'processing':
                        setStatus('processing');
                        setMessage('Su pago se está procesando. Le notificaremos cuando se haya completado.');
                        break;
                    case 'requires_payment_method':
                        setStatus('error');
                        setMessage('El pago falló. Por favor, intente con otro método de pago o verifique sus datos.');
                        break;
                    default:
                        setStatus('error');
                        setMessage('Algo salió mal. Por favor, contacte a soporte.');
                        break;
                }
            });
        });
    }, [searchParams, stripePromise]);

    const getStatusIcon = () => {
        switch(status){
            case 'success': return <span className="text-6xl text-green-500">✓</span>;
            case 'error': return <span className="text-6xl text-red-500">✗</span>;
            case 'processing':
            case 'loading':
            default: return <div className="w-12 h-12 border-4 border-blue-500 border-dashed rounded-full animate-spin"></div>;
        }
    }

    return (
        <div className="min-h-screen bg-gray-100 flex flex-col items-center justify-center p-4">
            <div className="w-full max-w-lg p-8 text-center bg-white rounded-lg shadow-xl">
                <div className="mb-6">
                    {getStatusIcon()}
                </div>
                <h1 className={`text-2xl font-bold mb-2 ${status === 'success' ? 'text-gray-800' : 'text-red-700'}`}>
                    {status === 'success' ? 'Pago Exitoso' : status === 'error' ? 'Error en el Pago' : 'Procesando Pago'}
                </h1>
                <p className="text-gray-600 mb-8">
                    {message}
                </p>
                <Link href="/">
                    <div className="inline-block px-6 py-3 bg-blue-600 text-white font-semibold rounded-lg shadow-md hover:bg-blue-700 transition-colors cursor-pointer">
                        Volver al inicio
                    </div>
                </Link>
            </div>
        </div>
    );
}

// Envolvemos el componente en Suspense para manejar los searchParams del lado del cliente
export default function PagoCompletadoPage() {
    return (
        <Suspense fallback={<div>Cargando...</div>}>
            <StatusDisplay />
        </Suspense>
    );
}
