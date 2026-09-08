# Estado de sesión — ClassicLightFX

- Fecha: 2026-09-08.
- Repo: `juanamores98/ClassicLightFX`, rama `main`.
- HEAD de partida: `da2cdfb58705d983203dd3e67735c469c49e6b43`.
- Cambios preparados para commit y push a main por autorización del usuario. Árbol de partida limpio; esta entrega conserva las pruebas offline y no instala el mod.
- Encargo: correcciones, VANILLA/OPTIMIZED y panel estrecho incrustable; MIT/MIT-0; sin integración.
- NeuralFX y SkyFX excluidos.

## Entregado

VANILLA es una suspensión persistente. OPTIMIZED desactiva todos los efectos clásicos, como el DEFAULT de referencia. ApplyOnLoad=false deja de activar la niebla. Se respeta al dueño activo de iluminación/niebla y se sustituye la tabla solar de ocho claves de procedencia incierta por una transformación del gradiente capturado.

Receta del perfil activo Default. SHA256 del archivo fuente: `71062f3ac84872ca305db47237e5fd7168bf5b5b629cfff9b24eae45ad6bbfba`. Referencia y limitaciones en `SceneFX/docs/VALIDACION.md`.

## Comprobado

- Build net35/C# 7.3: cero errores.
- Cuatro advertencias MSB3245: referencias implícitas System.Data, System.Drawing, System.Runtime.Serialization y System.Xml.Linq no resueltas en este entorno.
- Suite conjunta: **60 PASS, 0 FAIL**, con .NET 8 y dobles. Código y resultados en `SceneFX/tests/Regression`.
- Build sin instalación; DLL instaladas fuera de este cambio.

## Pendiente y próximo paso

El gradiente solar y las LUTs son aproximaciones propias; no se ha demostrado igualdad con la versión anterior a After Dark. Los objetivos históricos de potencia y tinte conservados tienen evidencia limitada, registrada en PROCEDENCIA.md. No afirmar paridad visual total ni certificación jurídica.

Prueba agrupada en el juego con los cuatro binarios de esta revisión, después de autorizar despliegue con respaldo y juego cerrado. Matriz en `SceneFX/docs/VALIDACION.md`: primera/segunda ciudad, reinicio, modos repetidos, LUT presente/ausente, UI estrecha, convivencia y comparación con DEFAULT. Registrar observaciones y medidas antes de aprobar integración o afirmar «100 %».
