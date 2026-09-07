# Procedencia de los valores clásicos

ClassicLightFX se distribuye bajo **MIT-0**. El motor del look clásico se reescribió desde cero
en `9a09472`, retirando el código derivado de Daylight Classic (GPL-3). Lo que quedó son
**valores numéricos**, y este documento registra de dónde sale cada uno, porque un valor sólo
puede conservarse bajo MIT-0 si es un hecho sobre el juego y no expresión ajena.

Verificado el 2026-09-06 contra la clase decompilada del propio juego,
`DayNightProperties`, en el expediente de auditoría.

## Verificados como datos del juego

| Valor | Dónde vive en v2 | Evidencia |
| --- | --- | --- |
| `Color32(55, 66, 77)` | `ClassicLook.cs` · `NightSunColor` | Aparece literalmente en la gradiente `m_LightColor` que trae la clase del juego, en el instante 0.23 |
| `Color32(245, 173, 84)` | `ClassicLook.cs` · `DawnSunColor` | Ídem, en 0.26 y en 0.74 |
| `Color32(252, 222, 186)` | `ClassicLook.cs` · `MorningSunColor` | Ídem, en 0.5 |
| Tiempos 0.23, 0.26, 0.74, 0.77 | `ClassicLook.cs` · `SunCurveTimes` | Ídem: son los instantes de la propia gradiente del juego |
| `(680, 680, 680)` | `ClassicFogDriver.cs` · `ClassicWavelengths` | El juego declara `m_WaveLengths = (680, 550, 440)`; la variante clásica iguala los tres canales al primero |
| Latitudes y longitudes por entorno | `ClassicLook.cs` · `TryGetCityCoordinates` | Coordenadas reales de Londres, Estocolmo, Malta y La Meca. Hechos geográficos |
| `m_Latitude` / `m_Longitude` por mapa | — | El juego los carga del *theme* del mapa; v2 sólo los sustituye y los repone |

Los colores individuales, las longitudes de onda y las coordenadas son **hechos**: describen
cómo se comporta el juego o dónde está una ciudad. No son expresión protegible, y conservarlos
bajo MIT-0 es correcto.

## Un valor que es un hecho pero no verificable aquí

`ClassicSunPower = 3.318695f` describe la intensidad solar de la versión anterior a After Dark.
El juego actual declara `m_SunIntensity = 1f`, así que **este número no se puede comprobar
contra la build instalada**: sólo contra una build previa, que ya no se distribuye.

Un número aislado que describe el estado anterior de un programa es un hecho, no autoría. Se
conserva. Lo mismo aplica al tinte atmosférico clásico `(104, 166, 211)`, que sustituye al
`m_SkyTint = (0.5, 0.5, 0.5)` actual.

## Lo que queda por decidir

**La disposición de ocho claves de `SunCurveTimes` / `SunCurveColors` no está verificada.**

La gradiente que trae el juego tiene **siete** claves y coloca `252,222,186` en 0.5. La curva
que usa v2 tiene **ocho**, mueve ese color a 0.29 y 0.71, y añade una meseta de blanco puro
(`255,255,255`) entre 0.35 y 0.65 que **no existe en la gradiente del juego actual**.

Caben dos lecturas y no se puede distinguir entre ellas sin una build anterior a After Dark:

1. Es la curva literal de aquella versión. Entonces es un hecho, como el resto de esta página.
2. Es la reconstrucción que hizo la comunidad de Daylight Classic. Entonces es lo único
   creativo que queda, y está copiado tal cual.

Los colores sueltos son datos del juego en cualquiera de los dos casos; lo que está en duda es
**el arreglo**. El riesgo es estrecho —ocho claves de una gradiente— pero es el último punto en
el que la etiqueta MIT-0 se apoya en una suposición.

**Salida barata, si se quiere cerrar del todo.** Construir la curva clásica *a partir de la
gradiente que ya se captura en `Baseline.SunGradient`*, con una transformación documentada
—aplanar el mediodía hacia el blanco, comprimir las bandas de crepúsculo— en vez de una tabla
literal. El resultado sería expresión propia y produciría un look equivalente. Cambia lo que se
ve, así que exige una comparación A/B en partida antes de adoptarlo, y por eso se deja
planteado y no aplicado.

## Las tablas de color

`ClassicLutSynth.cs` genera las cinco tablas por código, con constantes de respuesta propias.
No se incrusta, convierte ni ajusta ninguna textura de terceros. Las tablas originales del juego
se conservan intactas en `ModernTableBackup` y se reponen al desactivar la función.
