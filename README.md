# PruebaTecnica API

API REST en **.NET 10** con **Minimal API** para gestionar usuarios, sus direcciones y monedas con conversión de divisas.

**Stack:** Entity Framework Core 10 + SQLite · FluentValidation · CQRS · seguridad por API Key.

## Requisitos

- [.NET SDK 10.0.100](https://dotnet.microsoft.com/download/dotnet/10.0) o superior
- *(Opcional, solo para crear migraciones nuevas)* herramienta `dotnet-ef` 10.0.15

## Cómo correr el proyecto

```bash
git clone https://github.com/nelpar/prueba-tecnica-api.git
cd prueba-tecnica-api
dotnet run --project src/PruebaTecnica.Api
```

La API queda disponible en `http://localhost:5278`.

No hace falta configurar nada más: la base de datos se crea sola al iniciar (ver abajo).

## Base de datos (SQLite)

- **Creación automática:** al arrancar, la API ejecuta `Database.Migrate()`. Si el archivo `pruebatecnica.db` no existe, lo crea con todas las tablas y las monedas iniciales. Si ya existe, aplica solo las migraciones pendientes.
- **Ubicación:** el archivo se crea en el directorio desde el que se ejecuta la API (con `dotnet run --project ...` queda en `src/PruebaTecnica.Api/`). No se versiona (está en `.gitignore`).
- **Reiniciar la base:** borrar `pruebatecnica.db` y volver a levantar la API.

### Migraciones (manual)

```bash
# Instalar la herramienta (una vez por máquina)
dotnet tool install --global dotnet-ef --version 10.0.15

# Crear una migración nueva
dotnet ef migrations add <Nombre> -p src/PruebaTecnica.Infrastructure -s src/PruebaTecnica.Api -o Migrations

# Aplicar migraciones sin levantar la API
dotnet ef database update -p src/PruebaTecnica.Infrastructure -s src/PruebaTecnica.Api
```

### Monedas iniciales

| Código | Nombre | RateToBase |
|---|---|---|
| PYG | Guaraní paraguayo | 1 (moneda base) |
| USD | Dólar estadounidense | 7300 |
| EUR | Euro | 7900 |

Las tasas son **valores de prueba**. USD = 7300 reproduce el ejemplo del enunciado (100 USD → 730000 PYG).

## Autenticación (API Key)

Todos los endpoints requieren el header:

```
X-API-KEY: test-2004
```

La clave se configura en `src/PruebaTecnica.Api/appsettings.json` (propiedad `ApiKey`). Si el header falta o es incorrecto, la API responde **401 Unauthorized** en formato ProblemDetails.

```bash
curl -H "X-API-KEY: test-2004" http://localhost:5278/users
```

## Endpoints

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| POST | `/users` | Crear usuario | 201, 400, 409 |
| GET | `/users?isActive=true\|false` | Listar usuarios (filtro opcional) | 200 |
| GET | `/users/{id}` | Obtener usuario | 200, 404 |
| PUT | `/users/{id}` | Modificar name, email, isActive | 200, 400, 404, 409 |
| DELETE | `/users/{id}` | Eliminar usuario (y sus direcciones) | 204, 404 |
| POST | `/users/{userId}/addresses` | Crear dirección | 201, 400, 404 |
| GET | `/users/{userId}/addresses` | Listar direcciones del usuario | 200, 404 |
| PUT | `/addresses/{id}` | Modificar dirección | 200, 400, 404 |
| DELETE | `/addresses/{id}` | Eliminar dirección | 204, 404 |
| GET | `/currencies` | Listar monedas | 200 |
| POST | `/currencies` | Crear moneda | 201, 400, 409 |
| POST | `/currency/convert` | Convertir un monto entre monedas | 200, 400, 404 |

Todas responden **401** sin API Key válida.

### Ejemplos

**Crear usuario**
```http
POST /users
X-API-KEY: test-2004
Content-Type: application/json

{ "name": "Juan", "email": "juan@test.com", "password": "secreta123" }
```

**Crear dirección**
```http
POST /users/1/addresses
X-API-KEY: test-2004
Content-Type: application/json

{ "street": "Calle Falsa 123", "city": "Asunción", "country": "Paraguay", "zipCode": "9999" }
```

**Convertir divisas**
```http
POST /currency/convert
X-API-KEY: test-2004
Content-Type: application/json

{ "fromCurrencyCode": "USD", "toCurrencyCode": "PYG", "amount": 100 }
```
```json
{ "fromCurrency": "USD", "toCurrency": "PYG", "originalAmount": 100, "convertedAmount": 730000 }
```

### Formato de errores

Todos los errores siguen el estándar **ProblemDetails** (RFC 9457). Los errores de validación incluyen el detalle por campo:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": { "Email": ["El email no tiene un formato válido."] }
}
```

## Arquitectura

Clean Architecture en 4 proyectos:

```
src/
├── PruebaTecnica.Domain          Entidades (User, Address, Currency). Sin dependencias.
├── PruebaTecnica.Application     Casos de uso: Commands, Queries, Handlers, Validators, DTOs.
│   ├── Users/Commands · Users/Queries
│   ├── Addresses/Commands · Addresses/Queries
│   ├── Currencies/Commands · Currencies/Queries
│   └── CurrencyConversion
├── PruebaTecnica.Infrastructure  EF Core (DbContext, configuraciones, migraciones, seed) y hasheo de contraseñas.
└── PruebaTecnica.Api             Endpoints, middleware de API Key, filtro de validación, manejo global de errores.
```

**Flujo de un request:** middleware de API Key → filtro de validación (FluentValidation) → endpoint → handler (CQRS) → EF Core / SQLite. Las excepciones de negocio (`NotFoundException`, `ConflictException`) se traducen a 404 y 409 en un único manejador global.

## Decisiones de diseño

- **Contraseña en el alta de usuario.** El enunciado pide guardar la contraseña hasheada, pero el ejemplo de `POST /users` no la incluye. Se agregó `password` (mínimo 8 caracteres) al body. Se hashea con `PasswordHasher` de ASP.NET Core Identity (PBKDF2 con salt) y nunca se devuelve en las respuestas.
- **CQRS sin MediatR.** Se implementó con interfaces propias (`ICommandHandler`, `IQueryHandler`) registradas automáticamente por reflexión. MediatR pasó a licencia comercial en 2025, y para este alcance un dispatcher no aporta valor.
- **Validación de formato vs. reglas de negocio.** FluentValidation valida el formato del request (400). Las reglas que requieren consultar la base (usuario existente, email o código único) están en los handlers y responden 404 o 409.
- **Unicidad sin distinguir mayúsculas.** Email y código de moneda se normalizan (minúsculas y mayúsculas, respectivamente) y además tienen índice único con collation `NOCASE`.
- **El id siempre viene de la ruta.** En `PUT` y en el alta de direcciones, el id del body se ignora (`[JsonIgnore]`).
- **Borrado en cascada.** Eliminar un usuario elimina sus direcciones.
- **Dirección de un usuario inexistente → 404**, tanto al crear como al listar.
- **La conversión es una Query.** No modifica datos; usa `POST` porque así lo define el enunciado.
- **Redondeo de la conversión:** 4 decimales, redondeo comercial (`MidpointRounding.AwayFromZero`).
- **Códigos de moneda ISO 4217:** se validan como 3 letras.
- **`decimal` en SQLite.** SQLite no tiene tipo decimal; EF Core lo guarda como texto. Por eso nunca se ordena ni se filtra por `RateToBase` en la base, y los cálculos se hacen en memoria con `decimal`.

## Estado

**Implementado (todo lo pedido en el enunciado):**
- CRUD de Users con filtro `isActive`
- CRUD de Addresses (relación 1:N)
- Listado y alta de Currencies, y conversión de divisas
- Seguridad por API Key
- EF Core + SQLite con migraciones y datos iniciales
- FluentValidation en crear/editar usuario, crear/editar dirección, crear moneda y conversión
- Patrón CQRS con la estructura de carpetas pedida

**No implementado / mejoras posibles:**
- Tests automatizados (el proyecto `tests/PruebaTecnica.Tests` está creado y vacío)
- Documentación OpenAPI/Swagger
- Paginación en los listados
