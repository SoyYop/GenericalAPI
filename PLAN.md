# Plan de mejoras de pruebas genéricas

- [x] Reubicar utilidades de pruebas bajo `tests/GenericalAPI.Tests.Unit/TestUtils` y alinear los `using` a ese namespace.
- [x] Crear una clase base de pruebas para repositorios genéricos (`GenericRepositoryTestsBase<TEntity>`) que reciba el `DbContext` via `TestDatabaseHelper` y permita reutilizar casos entre `Product`, `Store`, etc.
- [x] Incorporar builders/fakes para entidades y DTOs (por ejemplo `ProductBuilder`, `StoreBuilder`) para minimizar duplicación de datos de prueba.
- [x] Añadir `IClassFixture` que gestione la fábrica de `AppDbContext` y permita alternar `InMemory`/`LocalDb` desde una sola configuración.
- [ ] Expandir README o `docs/` con la guía de cómo agregar nuevas pruebas genéricas (repos, servicios, controladores) usando las bases anteriores.
