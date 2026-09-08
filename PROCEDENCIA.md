# Procedencia y distribuciÃ³n â€” ClassicLightFX

ActualizaciÃ³n 2026-09-08. La licencia propia se mantiene en **MIT-0**; no se cambia autorÃ­a ni se reescribe el historial.

## Alcance de esta sesiÃ³n

Se corrigiÃ³ el cÃ³digo FX existente usando sus estructuras y las APIs de la instalaciÃ³n local de Cities: Skylines 1. Las tablas de comportamiento del encargo y de la auditorÃ­a guiaron los requisitos. No se incorporaron cuerpos de mÃ©todos, shaders ni texturas GPL a estas correcciones.

La auditorÃ­a previa de esta misma conversaciÃ³n recibiÃ³ fragmentos de implementaciÃ³n legada. **No se presenta esta sesiÃ³n como un cuarto limpio aislado ni como una certificaciÃ³n de independencia de toda la historia del repositorio.** La autorizaciÃ³n posterior del usuario permite corregir los FX existentes; no convierte cÃ³digo restringido en reutilizable.

| Pieza | Evidencia / revisiÃ³n | DecisiÃ³n | LÃ­mite o siguiente paso |
|---|---|---|---|
| CÃ³digo FX de partida | HEAD da2cdfb58705d983203dd3e67735c469c49e6b43 | Continuar implementaciÃ³n existente; cambios locales trazables | No certificar el historial completo |
| Cambios de persistencia, panel y modos | Diff 2026-09-08 | ImplementaciÃ³n sobre FX; helpers compartidos entre los cuatro repos propios | Mantener MIT-0 de contribuciones propias |
| Receta personal DEFAULT | XML activo y SHA256 en SceneFX/docs/Default.reference.txt | Datos de configuraciÃ³n, no implementaciÃ³n de RenderIt Plus | Modelos distintos identificados como aproximaciones |
| Assets LUT de Workshop | SelecciÃ³n por nombre del juego | No copiar ni empaquetar texturas | Cada recurso conserva sus condiciones |
| APIs del juego y Unity | DLL locales; build net35; firmas y unidades comprobadas | Referencias de compilaciÃ³n/runtime | No distribuir DLL del juego como parte del mod |
| Fuentes legadas | AuditorÃ­a de comportamiento conservada | No adoptar implementaciÃ³n GPL o sin permiso verificado | Un futuro trasplante de cÃ³digo exige licencia por pieza |

Las copias de Relight, Fog Controller y Eyecandy X usadas en la auditorÃ­a no permitieron acreditar una licencia permisiva. Daylight Classic se identificÃ³ como GPL-3. Play It! tiene MIT en la revisiÃ³n oficial `c42ad0424eef8b684505fbc74cfa7fd4d9881b66`; esta correcciÃ³n no incorpora su cÃ³digo. No se trata a los cinco como un bloque uniforme de GPL.

## Aspecto clÃ¡sico: cambio concreto

Se elimina la disposiciÃ³n de ocho claves solares cuya procedencia quedaba abierta en la versiÃ³n anterior de este documento. La nueva curva utiliza las claves del gradiente capturado del mapa, conserva sus tiempos y alfa, y aproxima un mediodÃ­a mÃ¡s neutro con una mezcla hacia blanco de hasta 35 %. Es una transformaciÃ³n propia documentada, **no la curva histÃ³rica acreditada de After Dark**.

Las LUTs estÃ¡ndar siguen sintetizÃ¡ndose mediante el cÃ³digo propio existente de ClassicLutSynth; no se incorporan texturas de Daylight Classic. Esas aproximaciones opcionales de aspecto clÃ¡sico no son la familia de LUTs Optimized que se eliminÃ³ de SceneFX.

Permanecen los objetivos heredados `ClassicSunPower=3.318695`, exposiciÃ³n 1 y tinte RGB 104/166/211. Su correspondencia exacta con una build anterior a After Dark **no se pudo comprobar contra la instalaciÃ³n actual**. Los canales de longitudes de onda y coordenadas se documentan como decisiones del control. Se retiran las afirmaciones jurÃ­dicas absolutas anteriores sobre estos datos: este archivo registra evidencia y lÃ­mites, no emite un dictamen.

Comparar A/B el nuevo gradiente antes de presentarlo como sustituto visual completo.
