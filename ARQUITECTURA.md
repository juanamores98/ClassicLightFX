# Arquitectura actual de ClassicLightFX

Revisión 2026-09-08. El historial conserva la descripción anterior de ventanas y presets.

## Recorrido

1. Entrada del mod y anfitrión: configuración global y servicios del nivel.
2. Modelo: `Options/ModOptions.cs` y su `OptionsDocument` temporal. El XML se lee en un documento temporal validado antes de copiarlo al estado vivo.
3. Motor: `Core/ClassicLook.cs`, `ClassicFogDriver.cs` y `ClassicLutSynth.cs`. Captura antes de escribir; limpia referencias y caches al descargar.
4. `FxModule` expone estado y panel. `UI/PanelView.cs` contiene disposición vertical, controles nativos y refresco sin escribir.
5. `Infrastructure/FxStorage.cs` proporciona escritura segura, validación de finitos y reconocimiento de la receta; `FxInterop.cs` consulta reclamaciones sin dependencia obligatoria del compañero.

VANILLA es una suspensión persistente. OPTIMIZED desactiva todos los efectos clásicos, como el DEFAULT de referencia. ApplyOnLoad=false deja de activar la niebla. Se respeta al dueño activo de iluminación/niebla y se sustituye la tabla solar de ocho claves de procedencia incierta por una transformación del gradiente capturado.

## Modos

VANILLA suspende el módulo y devuelve los campos escritos a su referencia previa cuando le corresponde. No equivale a aplicar constantes supuestamente neutras. Conserva el modo en el archivo global.

OPTIMIZED lee `BuiltIns/Optimized.xml`, incorporado en el ensamblado. La referencia seleccionada y diferencias están en `SceneFX/docs/Default.reference.xml` y `SceneFX/docs/VALIDACION.md`. No lee RenderIt Plus por frame ni lo integra.

`Mode` compara la receta contra el estado exportado: al editar muestra CUSTOM. Describe la configuración, no acredita disponibilidad del LUT ni ausencia de interferencias externas. `Status` comunica errores detectados.

## Contrato de incrustación

Llamadas de UI y motor en el hilo principal de Unity, con servicios del nivel disponibles:

```csharp
var panel = ClassicLightFX.FxModule.CreatePanel(parent, 320f, 650f);
string xml = ClassicLightFX.FxModule.ReadState();
bool accepted = ClassicLightFX.FxModule.ApplyState(xml);
panel.Refresh(); // refrescar tras modificaciones externas
panel.SetSize(320f, 700f);
panel.Dispose(); // destruir UI no desactiva la configuración
ClassicLightFX.FxModule.Flush();
```

- `parent` es un `ColossalFramework.UI.UIComponent` del futuro host. Este conserva la referencia, visibilidad y disposición.
- `Release()` es una acción separada: libera motor y persiste VANILLA.
- `ApplyState` recibe la sección XML exportada del mod. Rechaza raíz ajena, texto inválido y números no finitos. No aplica parcialmente una sección con validación fallida.
- La exportación no incluye la posición de ventana. Los campos de mundo tienen su alcance y modos explícitos.
- `ClassicLightFXMod.ActiveClaims` informa de campos compartidos; no bloquea físicamente escrituras del motor.
- Tamaño preferido 360×680 y mínimo 280×260. Reapertura independiente recoloca el panel dentro de la resolución actual.

Sin SDK global, RPC, plugins ni referencia a Arrebol/RenderIt Plus.

## Cooperación y guardado

Lumen tiene prioridad para la luz que reclama; Atmosphere, para la niebla cuando está activo. Scene delega ediciones de look a Lumen activo y aplica localmente cuando está suspendido. Classic consulta reclamaciones antes de escribir/restaurar. La carga automática de Scene no sustituye las preferencias globales guardadas de Lumen.

La coordinación cubre los FX revisados. Otros mods, el orden real de carga y cambios tardíos de tema requieren prueba dentro del juego; no se garantiza restauración universal.

Estado: ClassicLightFX2.xml. Cambios agrupados durante aproximadamente un segundo, escritura temporal, reemplazo con `.bak`, pendiente hasta éxito y flush al cerrar anfitrión. Carga inválida no copia los primeros campos al estado vivo. La recuperación de `.bak` es manual; no se implementa una migración universal de formatos legados.

## Verificación

[Estado de sesión](docs/ESTADO-SESION.md) y [paridad](docs/PARIDAD.md) distinguen controles, comportamiento, formatos y aspecto.

El gradiente solar y las LUTs son aproximaciones propias; no se ha demostrado igualdad con la versión anterior a After Dark. Los objetivos históricos de potencia y tinte conservados tienen evidencia limitada, registrada en PROCEDENCIA.md. No afirmar paridad visual total ni certificación jurídica.
