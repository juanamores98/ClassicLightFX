# Diseño de ClassicLightFX v2

Especificación funcional de la segunda versión.

## Objetivo

Restaurar de forma reversible el perfil visual clásico del juego (anterior a la
expansión After Dark) a partir de los valores y recursos propios del juego,
mediante un ciclo de vida limpio: aplicar al cargar el mapa, revertir al salir.

## Perfil clásico (datos del juego, no de terceros)

| Elemento | Valor clásico |
|---|---|
| LUTs por bioma | Temperate, European, Boreal, Tropical, Winter (embebidos como recursos del ensamblado) |
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

## Opciones (v2)

`swapLuts`, `sunColor`, `sunStrength`, `sunCoords`, `fogMode`, `fogTint`,
`applyOnLoad` — todas on por defecto, persistidas en `ClassicLightFX2.xml`
(raíz `classicLightFx`, `schema="2"`), con presets rápidos
"All classic" / "All modern".

## Arquitectura

- `Core/ClassicTweaks` — aplicación y reversión de cada función.
- `Core/LutLibrary` — carga de recursos embebidos y lectura del entorno.
- `Core/FogModeSync`, `Core/FogTintSync` — componentes por frame de la niebla.
- `Options/` — esquema XML v2 y panel.
- `Locale/Translator` — traducciones (EN, RU, KR, zh ×3).
