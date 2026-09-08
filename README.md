# ClassicLightFX

Controles reversibles de aspecto clÃ¡sico para Cities: Skylines 1: iluminaciÃ³n, posiciÃ³n solar, niebla y aproximaciones propias de LUTs estÃ¡ndar.

## Uso

- Abrir con **F9**, o UUI opcional.
- Panel nativo preferido: **360 Ã— 680**, mÃ­nimo 280 Ã— 260. Secciones: Light, Fog.
- **VANILLA** libera las modificaciones del mÃ³dulo y guarda ese modo. Restaura la referencia capturada respetando compaÃ±eros detectados; un tema u otro mod puede hacer que difiera del vanilla puro.
- **OPTIMIZED** aplica la parte de este mÃ³dulo del **Default personal de RenderIt Plus**. Es una receta de aspecto, no de rendimiento.
- Editar controles guarda un estado personalizado. El pie distingue VANILLA, OPTIMIZED y CUSTOM segÃºn los valores configurados.
- Los sliders incluyen entrada decimal y refresco sin escrituras por repaint.

Todos los efectos clÃ¡sicos desactivados. Esto es deliberado: tu DEFAULT tiene sus interruptores clÃ¡sicos apagados. Para explorar el aspecto clÃ¡sico se conservan las casillas y la acciÃ³n Â«Enable all classic featuresÂ».

## Persistencia

Archivos globales: **ClassicLightFX2.xml**, bajo `%LOCALAPPDATA%\Colossal Order\Cities_Skylines`. Independientes de la partida. Temporal y reemplazo con copia `.bak`, pendientes que se reintentan y guardado al cerrar el anfitriÃ³n. Se conserva lectura desde la ubicaciÃ³n histÃ³rica cuando procede. Un cierre forzado durante el intervalo de guardado puede perder el Ãºltimo cambio pendiente.

Los built-ins no sobrescriben presets ya extraÃ­dos del usuario. Los botones de modo leen la receta incorporada; un antiguo archivo llamado Optimized puede contener valores distintos.

## IncrustaciÃ³n futura

`FxModule.CreatePanel(parent, width, height)` crea el panel dentro de un `UIComponent`. Con padre no tiene arrastre ni botÃ³n de ventana. Ofrece `ReadState`, `ApplyState`, `Release`, `ApplyOptimized`, `Flush`, `Mode` y `Status`. Ver [arquitectura](ARQUITECTURA.md).

No se ha integrado con RenderIt Plus ni Arrebol; tampoco hay dependencia de esos productos.

## Compilar y verificar

```powershell
dotnet build ClassicLightFX.csproj -c Release
```

Target **net35 / C# 7.3**, referencias de CS1 instalado. El build normal genera `bin/Release/net35` y **no instala**. El target de despliegue requiere `DeployMod=true`; solo debe utilizarse con autorizaciÃ³n, juego cerrado y respaldo.

Regresiones conjuntas: `SceneFX/tests/Regression/Regression.csproj`, que enlaza el cÃ³digo actual de los cuatro repos hermanos. Prueba lÃ³gica en .NET 8 con dobles del motor; no valida render ni interacciÃ³n visual.

## LÃ­mites

El gradiente solar y las LUTs son aproximaciones propias; no se ha demostrado igualdad con la versiÃ³n anterior a After Dark. Los objetivos histÃ³ricos de potencia y tinte conservados tienen evidencia limitada, registrada en PROCEDENCIA.md. No afirmar paridad visual total ni certificaciÃ³n jurÃ­dica.

[Paridad](docs/PARIDAD.md) Â· [Estado](docs/ESTADO-SESION.md) Â· [Procedencia](PROCEDENCIA.md). `DESIGN.md` se conserva como referencia histÃ³rica.

CÃ³digo propio bajo **MIT-0**, [LICENSE](LICENSE).


## Consolidación 2.1

Cambios de propiedad, Game, presets y convivencia: [contrato v3 y pruebas](docs/CONSOLIDACION-v3.md). Actualizar los cuatro FX juntos para usar suites.
