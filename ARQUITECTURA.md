# ClassicLightFX — arquitectura y cambios

Documento ejecutivo. Estado a día de hoy. `DESIGN.md` es la especificación
funcional original y `PROCEDENCIA.md` la evidencia de origen de cada pieza; este
documento describe cómo está construido el mod hoy y qué cambió en el último
ciclo.

## Qué hace

Devuelve el aspecto de Cities: Skylines anterior a *After Dark* (2015): las
tablas de color de entonces, el color y la intensidad del sol de entonces, sus
coordenadas, y el shader de niebla clásico. Cada pieza es un interruptor
independiente, y las tres combinaciones que la gente suele querer están como
botones de un clic.

## Piezas

```
Source/
  ClassicLightFXMod.cs        IUserMod + API de suite (8 etiquetas)
  Core/
    ClassicLook.cs            el conmutador: una función por característica
    ClassicFogDriver.cs       decide cuándo vale el efecto de niebla clásico
    ClassicLutSynth.cs        sintetiza las tablas, no las copia
    ClassicEngine.cs          MonoBehaviour anfitrión, F9
    LutLibrary.cs             tablas por bioma
    ThemeOwnership.cs         cede a Theme Mixer lo que es suyo
    QuickPresets.cs           Vanilla y Optimized de un clic
  Options/
    ModOptions.cs             el estado, público para que XmlSerializer lo vea
    OptionsPanel.cs           opciones dentro del menú del juego
  UI/ClassicWindow.cs         ventana IMGUI
  Locale/                     traducciones
```

### Las tablas son sintetizadas, no copiadas

`ClassicLutSynth` **genera** las tablas de color con curvas propias en lugar de
incluir las del juego de 2015. No hay ni un byte ajeno en el repositorio, que es
lo que permite que este mod sea MIT-0. Las tablas se generan con
`wrapMode = Clamp` y `filterMode = Bilinear`; sin eso, los extremos del rango
se envolvían y aparecían franjas de color.

`LutLibrary` elige la tabla según el bioma del mapa. Un bioma que no conozca se
deja como está: es mejor no tocar que aplicar una tabla equivocada.

### Cuándo vale la niebla clásica

`ClassicFogDriver` decide entre el efecto clásico y el moderno:

```csharp
bool allowWithCycle = ModOptions.Instance.ClassicFogWithCycle;
bool useLegacy = wantsClassic && (allowWithCycle || !cycleEnabled || !night);
```

`ClassicFogWithCycle` era el único ajuste de Daylight Classic que no tenía
equivalente aquí, y estaba fijo en «no»: de noche y con el ciclo día/noche
activo, la niebla clásica se apagaba sola sin que nadie pudiera evitarlo.

## Los tres looks de un clic

| Botón | LUTs | Sol | Niebla clásica | Tinte | Con ciclo |
|---|---|---|---|---|---|
| **Pre-AD (2015)** | sí | color, fuerza y coordenadas | sí | sí | no |
| **Hybrid** | sí | color, fuerza y coordenadas | no | sí | sí |
| **Modern (2026)** | no | no | no | no | no |

*Hybrid* es la combinación útil que no era obvia: la luz y el sol de 2015 con la
niebla moderna, que sí sigue el ciclo día/noche.

## API de suite

`ApplySuiteSection` / `ExportSuiteSection`, públicas y estáticas. 8 etiquetas,
todas las que se aplican se exportan:

```
swapLuts sunColor sunStrength sunCoords
classicFogMode classicFogTint classicFogWithCycle applyOnLoad
```

## Dónde guarda las cosas

`%LOCALAPPDATA%\Colossal Order\Cities_Skylines\ClassicLightFX2.xml`.

`ModOptions` es **pública** a propósito: `XmlSerializer` exige que el tipo
serializado y todos los que lo contienen lo sean. Mientras fue interna, el mod
no persistía ni una opción y nadie lo notaba porque el error se tragaba en
silencio.

## Qué cambió en este ciclo

**`ClassicFogWithCycle`**, el ajuste que faltaba para tener paridad completa con
Daylight Classic.

**Los tres looks de un clic** delante de los interruptores granulares, que
siguen todos disponibles debajo.

## Correcciones de la revisión

- **Sin emoji.** La fuente Arial de Unity 5.6 no lleva pictogramas. Comprobado
  sobre 12.960 ficheros `.cs` de mods que ya funcionan: ninguno los usa.
- **La ventana vuelve a estar entera en inglés.** Se habían colado siete
  literales en castellano junto a los controles en inglés que nadie tocó.

## Atajos

- `F9` — ventana del mod.

## Licencia

MIT-0 © 2026 juanamores98. Sin atribución ni condiciones.

Este mod **no contiene código ni datos de Daylight Classic** ni de ningún otro
mod. Reproduce el comportamiento contra los campos públicos del juego; el
detalle está en `PROCEDENCIA.md`.
