# Paridad de ClassicLightFX

Revisión: 2026-09-08. Requisitos del encargo y de la matriz de comportamiento de `ModdingResearch/EncargosFX/Auditoria-20260908/INFORME.md`. No implica adopción de implementaciones GPL.

| Capacidad | Fuente de requisito | Entrada / unidad / rango | Comportamiento | API comprobada | Destino | Prueba | Estado / diferencia |
|---|---|---|---|---|---|---|---|
| LUTs estándar | Daylight Classic | Interruptor | Generar aproximaciones propias y restaurar referencias previas | RenderProperties / Texture3DWrapper | ClassicLutSynth / ClassicLook | Build + revisión | No son texturas históricas ni LUTs Optimized de Scene |
| Color solar | Daylight Classic / revisión de procedencia | Interruptor | Transformación del gradiente del mapa; no tabla literal de ocho claves | DayNightProperties.m_LightColor | BuildClassicSunCurve | Build + inspección | Comparación visual pendiente |
| Potencia/exposición solar | Daylight Classic | Interruptor; objetivos heredados documentados | Ceder a Lumen/Scene activos; restituir solo si se escribió | DayNightProperties | ClassicLook | X01 | Coordinación lógica; objetivos históricos no revalidados |
| Coordenadas solares | Daylight Classic | Por entorno | Aplicar coordenadas y ceder a Scene cuando las controla | m_Latitude / m_Longitude | ClassicLook | Build + inspección | Probar cambios de tema y secuencias de activación |
| Niebla clásica y ciclo | Daylight Classic / H08 | Interruptor + permitir con ciclo | Efecto clásico solo de día; con ciclo requiere permiso explícito | FogEffect / DayNightFogEffect | ClassicFogDriver | C02, C03: 8 combinaciones | Corregido en prueba de lógica |
| Tinte atmosférico | Daylight Classic | Interruptor | Aplicación/restauración de tinte y longitudes de onda | DayNightProperties | ClassicFogDriver | Inspección + build | A/B y transición pendientes |
| Modos y archivo global | Encargo / H06–H09 | VANILLA / OPTIMIZED / casillas | Inicio nuevo en VANILLA; DEFAULT con efectos clásicos apagados | XML / snapshots | ModOptions / QuickPresets | C01, C02, C04, C05 | Sin equivalencia automática entre clásico y DEFAULT |

Las pruebas citadas están en `SceneFX/tests/Regression/Program.cs`. Firmas compiladas contra DLL reales; aserciones ejecutadas con dobles, no Unity. UI, imagen, tiempo de respuesta y rendimiento pendientes de observación.

**No se declara paridad total.** El gradiente solar y las LUTs son aproximaciones propias; no se ha demostrado igualdad con la versión anterior a After Dark. Los objetivos históricos de potencia y tinte conservados tienen evidencia limitada, registrada en PROCEDENCIA.md. No afirmar paridad visual total ni certificación jurídica.
