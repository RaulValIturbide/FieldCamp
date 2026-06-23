# Field Camp

> Un mod para **Mount & Blade II: Bannerlord** que te deja montar un campamento en cualquier punto del mapa de campaña y aprovechar el tiempo entre batallas.

![Bannerlord](https://img.shields.io/badge/Mount%20%26%20Blade%20II-Bannerlord-orange)
![Versión](https://img.shields.io/badge/versión-1.0.0-blue)
![Licencia](https://img.shields.io/badge/licencia-MIT-green)

---

## 📖 Descripción

**Field Camp** añade un icono de campamento al mapa de campaña y abre un menú cuyo fondo cambia según tu cultura y el terreno que te rodea. Con el campamento montado, tus hombres esperan tus órdenes: pueden entrenar, salir a forrajear o prepararse para una emboscada.

Cada actividad es **excluyente**: tu campamento hace una cosa a la vez. Tu progreso y el estado de las zonas forrajeadas se guardan con tu partida.

---

## ✨ Características

### ⚔️ Entrenar tropas
Tus soldados ganan experiencia de forma pasiva mientras mantienes la posición. Tu habilidad de **Liderazgo** determina cuánto aprenden, y a cambio tú ganas experiencia de Liderazgo según el tier y el número de soldados sanos a tu mando.

- Entrenar consume **comida extra** mientras tus hombres se ejercitan.
- Cada sesión puede salir **bien o mal** según tu Liderazgo y la moral del grupo.
- Algún soldado puede resultar **herido** durante el entrenamiento.
- Necesitas al menos un par de soldados en pie y **no puedes entrenar en el mar**.

<details>
<summary>📐 Fórmulas</summary>

```
XP por hora de tropa = 3 + redondear(0.4 × √Liderazgo)

XP de Liderazgo por hora del jugador = redondear(0.05 × Σ aporte)
  donde, por cada tropa que aún puede mejorar:
    aporte += soldados_sanos × tier

Probabilidad de éxito = 0.65 + (Liderazgo × 0.01) − (tropas × 0.01)
  mínimo 65%
```
</details>

### 🌾 Forrajear en busca de provisiones
Envía a tus hombres a buscar comida por los alrededores. Lo que traen depende del **terreno** en el que acampes —pescado en la costa, caza en los bosques, grano en las llanuras, dátiles en el desierto y más—, además del número de soldados y tu habilidad de **Explotación (Scouting)**.

- Si esquilmas una zona, se **agotará un tiempo**, así que tendrás que moverte y dejar que se recupere.
- Más Explotación y más soldados sanos = más comida por recolecta.

<details>
<summary>📐 Fórmulas</summary>

```
Cantidad de comida = redondear(0.12 × √(soldados_sanos × Explotación))
  siempre al menos 1, con un máximo de 4 por recolecta

Se intenta forrajear cada 4 horas. Tras esquilmar una zona,
ese lugar no volverá a dar comida durante unos 2 días mientras se recupera.
```
</details>

### 🥷 Preparar una emboscada
Oculta a tu grupo y desaparece del mapa, esquivando a los enemigos que merodean. El éxito depende de tu **Táctica** y tu **Subterfugio (Roguery)**.

- Si lo logras, permaneces oculto hasta que levantes el campamento.
- Si fallas, tendrás que esperar antes de volver a intentarlo.

<details>
<summary>📐 Fórmulas</summary>

```
Probabilidad de ocultarse = 0.40 + (Táctica × 0.002) + (Subterfugio × 0.002)
  con un máximo del 90%

Si fallas un intento, tendrás que esperar unas horas antes de volver a intentarlo.
```
</details>

---

## 📥 Instalación

### Manual
1. Descarga la última versión desde la sección [Releases](../../releases).
2. Extrae la carpeta `FieldCamp` en:
   ```
   ...\Mount & Blade II Bannerlord\Modules\
   ```
3. Inicia el launcher de Bannerlord y activa **Field Camp** en la pestaña de mods.
4. ¡A jugar!

### Vortex
Instala el archivo directamente desde Vortex; la extensión de Bannerlord detectará el `SubModule.xml` automáticamente.

---

## 🔧 Compatibilidad

- **Versión del juego:** *(1.2.4,1.4.5,1.4.6)*
- Compatible con partidas existentes.
- Se recomienda colocarlo **después** de los módulos nativos en el orden de carga.

---

## 🛠️ Compilar desde el código fuente

> Solo necesario si quieres modificar el mod o compilarlo tú mismo.

**Requisitos:**
- .NET / Visual Studio 2022 (o equivalente)
- Una instalación de Bannerlord (para las referencias a las DLL del juego)

**Pasos:**
1. Clona el repositorio:
   ```bash
   git clone https://github.com/RoboRaul/FieldCamp.git
   ```
2. Abre la solución en Visual Studio.
3. Ajusta las rutas de referencia a las DLL del juego si fuera necesario.
4. Compila en modo **Release**.
5. La DLL resultante se generará en `bin\Win64_Shipping_Client\`.

---

## 📁 Estructura del módulo

```
FieldCamp/
├── SubModule.xml                          # Manifiesto del mod
├── bin/
│   └── Win64_Shipping_Client/
│       └── FieldCamp.dll                  # Código compilado
├── ModuleData/                            # Datos XML, localización...
└── GUI/                                   # Prefabs, sprites, brushes...
```


## 🤝 Contribuir

Este es mi primer mod y tengo intención de seguir mejorándolo. Toda opinión, buena o mala, es bienvenida. Puedes:

- Abrir un [issue](../../issues) para reportar bugs o sugerir ideas.
- Enviar un *pull request* con mejoras.

---

## ❤️ Créditos y agradecimientos

- **RoboRaul** — desarrollo del mod.
- **[@Ser_Val](https://steamcommunity.com/id/Ser_Val/)** — por la chulísima imagen del mod.

Si quieres apoyar este mod o los que están por venir:

[☕ Buy me a coffee](https://buymeacoffee.com/roboraul)

---

## 📜 Licencia

Distribuido bajo licencia MIT. Consulta el archivo [`LICENSE`](LICENSE) para más detalles.
