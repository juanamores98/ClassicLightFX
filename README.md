# ClassicLightFX v2

Recupera el aspecto clÃ¡sico de **Cities: Skylines** anterior a After Dark: LUTs,
luz solar, posiciÃ³n del sol y niebla. Desarrollo original de **juanamores98**.

## QuÃ© hace

**Classic lighting**:
- *Stock LUT swap*: sustituye los LUTs del juego por los clÃ¡sicos (Temperate,
  European, Boreal, Tropical, Winter), embebidos en el DLL.
- *Classic sun color*: gradiente de luz solar con mediodÃ­a blanco.
- *Classic sun strength*: intensidad y exposiciÃ³n de la era pre-After Dark.
- *Classic sun position*: coordenadas reales por entorno del mapa
  (Londres/Europa, Estocolmo/Norte, Malta/Sunny, La Meca/Tropical).

**Classic fog**:
- *Classic fog mode*: alterna el efecto de niebla clÃ¡sico y el moderno segÃºn la
  hora y el ciclo dÃ­a/noche.
- *Classic fog tint*: tinte de cielo y longitudes de onda de dispersiÃ³n
  clÃ¡sicos, evaluados por hora del dÃ­a.

**General** (nuevo en v2):
- *Apply when a map loads*: activar/desactivar la aplicaciÃ³n automÃ¡tica.
- Presets rÃ¡pidos "All classic" / "All modern" y persistencia en `ClassicLightFX2.xml`.

## Compatibilidad

- Cities: Skylines **con After Dark** (el mod restaura el aspecto anterior a AD;
  sin AD muchos ajustes no tienen efecto).
- Se aplica al cargar el mapa y se revierte al descargarlo.
- Puede solaparse con otros mods de iluminaciÃ³n: gana el Ãºltimo en escribir.

## Requisitos

- **No requiere Harmony** (no parchea mÃ©todos: escribe valores y aÃ±ade/quita
  componentes de render).
- Carpeta `Locale/` junto al DLL (incluida) para las traducciones.
- Compila contra .NET Framework 3.5 (el runtime Mono del juego lo provee).

## InstalaciÃ³n

Copiar `ClassicLightFX.dll` y la carpeta `Locale/` a:

```
%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\ClassicLightFX\
```

## CompilaciÃ³n

```
dotnet build -c Release
```

El build despliega DLL + `Locale/` automÃ¡ticamente.

## Licencia

[MIT-0](https://spdx.org/licenses/MIT-0.html) (MIT No Attribution) Â© 2026 juanamores98.
Uso, copia, modificaciÃ³n, venta, distribuciÃ³n y sublicencia sin atribuciÃ³n ni condiciones.
