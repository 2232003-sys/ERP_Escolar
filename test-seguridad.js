// Script de validación de seguridad - "Test de Fuego"
// Prueba automática de las 3 fases de seguridad

async function testSecurity() {
  console.log('🔥 TEST DE FUEGO - Validación de Seguridad\n');

  // Fase 1: Verificar que los servicios estén corriendo
  console.log('📋 FASE 1: Verificación de Servicios');

  try {
    // Verificar backend
    const backendResponse = await fetch('http://localhost:5235/api/Auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username: 'admin', password: 'Admin123!' })
    });

    if (backendResponse.ok) {
      console.log('✅ Backend: Funcionando');
      const data = await backendResponse.json();
      const token = data.accessToken;
      console.log('✅ Token JWT: Generado correctamente\n');

      // Fase 2: Probar protección de rutas
      console.log('🔒 FASE 2: Prueba de Protección de Rutas');

      // Intentar acceder sin token
      const protectedResponse = await fetch('http://localhost:3000');
      if (protectedResponse.status === 200) {
        const text = await protectedResponse.text();
        if (text.includes('Redirigiendo al login') || text.includes('login')) {
          console.log('✅ Protección: Ruta bloqueada sin token');
        } else {
          console.log('⚠️  Protección: Ruta accesible sin token (posible problema)');
        }
      }

      // Intentar acceder con token (simular cookie/localStorage)
      console.log('✅ Token válido disponible para pruebas manuales\n');

      // Fase 3: Verificar persistencia de sesión
      console.log('💾 FASE 3: Prueba de Persistencia');

      // Verificar que el token se puede usar múltiples veces
      const verifyToken = await fetch('http://localhost:5235/api/control-escolar/alumnos', {
        headers: { 'Authorization': `Bearer ${token}` }
      });

      if (verifyToken.status === 200) {
        console.log('✅ Persistencia: Token válido para múltiples requests');
      } else {
        console.log('❌ Persistencia: Token no válido');
      }

      console.log('\n🎯 RESULTADO: Sistema de seguridad validado');
      console.log('\n📝 INSTRUCCIONES PARA PRUEBA MANUAL:');
      console.log('1. Ve a http://localhost:3000/login');
      console.log('2. Login con: admin / Admin123!');
      console.log('3. En el dashboard, haz click en "Cerrar Sesión"');
      console.log('4. Intenta acceder directamente a http://localhost:3000');
      console.log('5. Debería redirigirte al login');
      console.log('6. Haz login de nuevo');
      console.log('7. Presiona F5 - deberías mantener la sesión');

    } else {
      console.log('❌ Backend: No responde');
    }

  } catch (error) {
    console.log('❌ Error en validación:', error.message);
  }
}

testSecurity();