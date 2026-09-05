# ClassicLightFX v2

Recupera el aspecto clásico de **Cities: Skylines** anterior a After Dark: LUTs,
luz solar, posición del sol y niebla. Desarrollo original de **juanamores98**.

## Qué hace

**Classic lighting**:
- *Stock LUT swap*: sustituye los LUTs del juego por los clásicos (Temperate,
  European, Boreal, Tropical, Winter), embebidos en el DLL.
- *Classic sun color*: gradiente de luz solar con mediodía blanco.
- *Classic sun strength*: intensidad y exposición de la era pre-After Dark.
- *Classic sun position*: coordenadas reales por entorno del mapa
  (Londres/Europa, Estocolmo/Norte, Malta/Sunny, La Meca/Tropical).

**Classic fog**:
- *Classic fog mode*: alterna el efecto de niebla clásico y el moderno según la
  hora y el ciclo día/noche.
- *Classic fog tint*: tinte de cielo y longitudes de onda de dispersión
  clásicos, evaluados por hora del día.

**General** (nuevo en v2):
- *Apply when a map loads*: activar/desactivar la aplicación automática.
- Presets rápidos "All classic" / "All modern" y persistencia en `ClassicLightFX2.xml`.

## Compatibilidad

- Cities: Skylines **con After Dark** (el mod restaura el aspecto anterior a AD;
  sin AD muchos ajustes no tienen efecto).
- Se aplica al cargar el mapa y se revierte al descargarlo.
- Puede solaparse con otros mods de iluminación: gana el último en escribir.

## Requisitos

- **No requiere Harmony** (no parchea métodos: escribe valores y añade/quita
  componentes de render).
- Carpeta `Locale/` junto al DLL (incluida) para las traducciones.
- Compila contra .NET Framework 3.5 (el runtime Mono del juego lo provee).

## Instalación

Copiar `ClassicLightFX.dll` y la carpeta `Locale/` a:

```
%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\ClassicLightFX\
```

## Compilación

```
dotnet build -c Release
```

El build despliega DLL + `Locale/` automáticamente.

## Licencia

[MIT-0](https://spdx.org/licenses/MIT-0.html) (MIT No Attribution) © 2026 juanamores98.
Uso, copia, modificación, venta, distribución y sublicencia sin atribución ni condiciones.
