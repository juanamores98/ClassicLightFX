# Diseño de ClassicLightFX v2

Especificación funcional de la segunda versión.

## Objetivo

Restaurar de forma reversible el perfil visual clásico del juego (anterior a la
expansión After Dark) a partir de los valores y recursos propios del juego,
mediante un ciclo de vida limpio: aplicar al cargar el mapa, revertir al salir.

## Perfil clásico (datos del juego, no de terceros)

| Elemento | Valor clásico |
|---|---|
| LUTs por bioma | Temperate, European, Boreal, Tropical, Winter (sintetizadas proceduralmente en `ClassicLutSynth`; sin recursos externos embebidos) |
| Intensidad solar | 3.3187 |
| Exposición | 1.0 |
| Coordenadas por entorno | Europa → Londres; Norte → Estocolmo; Sunny → Malta; Tropical → La Meca |
| Tinte de cielo clásico | (104, 166, 211) / 255 |
| Longitudes de onda | 680 nm uniforme |

## Comportamiento

- **Ciclo de vida**: `OnLevelLoaded` → aplicar perfil (si `applyOnLoad`);
  `OnLevelUnloading` → revertir todo y destruir los componentes auxiliares.
- **Intercambio de LUTs**: se sustituyen las entradas del gestor de corrección
  de color del juego por wrappers propios y se fuerza el rebind del pipeline.
- **Modo de niebla**: un componente por frame decide qué efecto de niebla
  (clásico o moderno) está activo según hora del día, ciclo día/noche y opción
  del usuario; al destruirse restaura el moderno.
- **Tinte de niebla**: gradiente propio con banda clásica (0.35–0.65 del día)
  y valores modernos fuera de ella, evaluado por la hora normalizada del juego.
- Los valores "modernos" se capturan una sola vez al iniciar y se restauran al
  desactivar cada función.

## Opciones (v2.1)

`swapLuts`, `sunColor`, `sunStrength`, `sunCoords`, `fogMode`, `fogTint`,
`applyOnLoad` — todas on por defecto, persistidas en `ClassicLightFX2.xml`
(raíz `classicLightFx`, `schema="2"`), con presets rápidos
"All classic" / "All modern".

- **Rendimiento v2.1**: Se eliminó la escritura incondicional de XML por frame en `ClassicWindow.DrawWindow`. Las operaciones de guardado ahora están estranguladas (throttle de 1.0 s) y solo se ejecutan cuando un control cambia de estado o se cierra la ventana.
- **Posición de ventana**: Coordenadas `windowX` y `windowY` persistidas en el XML para restaurar la posición en pantalla del panel rápido F9 con verificación de límites.
- **Identidad visual unificada**: Cabeceras con color de acento `#4FC3F7`, márgenes y espaciados estandarizados compatibles con la suite.

### Integración de Suite

- Entry point expone `public static bool ApplySuiteSection(string xml)` y `public static string ExportSuiteSection()`.
- Permite a SceneFX activar o desactivar en un clic la restauración clásica al aplicar un perfil unificado `.suite.xml`.

## Arquitectura

- `Core/ClassicLook` — motor propio de la restauración clásica: captura un
  baseline del estado del juego y conmuta cada función entre valores modernos
  y valores del juego pre-After Dark (hechos del juego, no material de
  terceros). Implementación clean-room de v2.1: estructura, nombres y
  descomposición originales; sin recursos ni código derivado.
- `Core/ClassicLutSynth` — tablas de color clásicas sintetizadas por código.
- `Core/LutLibrary` — envoltura de las tablas sintetizadas y lectura del entorno.
- `Core/ClassicFogDriver` — componente por frame de la niebla (selección de
  efecto y tinte clásico).
- `Options/` — esquema XML v2 con throttle y panel.
- `UI/ClassicWindow` — panel rápido F9 con estilo unificado y posición persistida.
- `Locale/Translator` — traducciones (EN, RU, KR, zh ×3).

