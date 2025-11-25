## GenericalAPI

Estructura base del proyecto organizada en capas y pruebas.

```
GenericalAPI/
├── src/
│   ├── GenericalAPI.Api/              → API (Controllers, Minimal APIs, DI, Middlewares)
│   ├── GenericalAPI.Application/      → Lógica de negocio (Services, DTOs, Validators)
│   ├── GenericalAPI.Domain/           → Entidades, interfaces, reglas puras
│   ├── GenericalAPI.Infrastructure/   → EF Core / Repositorios / Integraciones externas
│   └── GenericalAPI.Shared/           → Utilidades, excepciones comunes, helpers
├── tests/
│   ├── GenericalAPI.Tests.Unit/       → xUnit: Application + Domain
│   ├── GenericalAPI.Tests.Integration/→ API + EF Core SQLite
│   └── GenericalAPI.Tests.EndToEnd/   → Pruebas completas de API
├── build/                             → Scripts CI/CD, pipelines, Sonar
└── docs/                              → Diagramas, OpenAPI, instrucciones
```

Puntos siguientes sugeridos:
- Agregar referencias entre proyectos (API → Application → Domain, etc.).
- Configurar proyectos de pruebas para apuntar a las capas correspondientes.
- Completar `build/` y `docs/` con scripts y documentación.
