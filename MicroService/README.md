# Challenge Prueba Tecnica
# 🛠 Sistema de Notificaciones de Inventario

Este proyecto implementa dos microservicios en .NET que se comunican a través de RabbitMQ para manejar eventos de inventario, con almacenamiento de logs en base de datos.

---

## 📦 Servicios

### 1. **InventoryService (Productor)**
API REST que permite crear, actualizar y eliminar productos. Publica eventos a RabbitMQ.

### 2. **NotificacionesService (Consumidor)**
Escucha los eventos publicados en RabbitMQ y guarda logs de los eventos en base de datos SQL Server.

---

## 🧰 Tecnologías utilizadas

- .NET 8
- ASP.NET Core Web API
- RabbitMQ
- Entity Framework Core
- SQL Server
- Docker + Docker Compose
- Polly (para resiliencia)
- RabbitMQ Management UI (`http://localhost:15672`, user/pass: guest)

---

## 🚀 Cómo ejecutar el proyecto

1. Clonar el repositorio

```bash
git clone https://github.com/tuusuario/nombre-repo.git
cd nombre-repo
```

2. Levantar con Docker Compose

```bash
docker-compose up --build
```

3. Acceder a los servicios:

| Servicio              | URL                           |
|-----------------------|-------------------------------|
| Inventory API         | http://localhost:8001/swagger |
| RabbitMQ UI           | http://localhost:15672        |
| NotificacionesService |                               |

---

## 🧪 Probar funcionalidad

### Crear producto (POST):
```http
POST http://localhost:8001/api/products
Content-Type: application/json

{
  "name": "Producto Test",
  "description": "Descripción de prueba",
  "price": 100.0,
  "stock": 10,
  "category": "Electrónica"
}
```

### Modificar producto (PUT):
```http
PUT http://localhost:8001/api/products/{id}
Content-Type: application/json

{
  "name": "Producto Test",
  "description": "Descripción de prueba actualizada",
  "price": 100.0,
  "stock": 10,
  "category": "Electrónica"
}
```

### Eliminar producto (DELETE):
```http
DELETE http://localhost:8001/api/products/{id}
```

### Buscar todos los productos (GET):
```http
GET http://localhost:8001/api/products
```

### Buscar un producto por su ID (GET):
```http
GET http://localhost:8001/api/products/{id}
```
Verificá luego en **RabbitMQ → Queues** o en la base `NotificationDb` que el evento se haya registrado.

---

## 🛡 Resiliencia

El proyecto implementa resiliencia con `Polly`:

- ✅ **Circuit Breaker**: Se activa luego de 2 errores consecutivos al intentar publicar en RabbitMQ.
- 🔄 El circuito se abre por 15 segundos para evitar saturar el sistema y se restablece automáticamente si RabbitMQ vuelve a estar disponible.

---

## 🗂 Estructura del Proyecto

```
📁 InventoryService
├── Context
├── Controllers
├── Dtos
├── HandlerExcepction
├── Messaging (con RabbitPublisher)
├── Migrations
├── Models
├── Repositories
    ├── Interfaces
    └── Impl
├── Services
    ├── Interfaces
    └── Impl
├── DockerFile
└── Program.cs

📁 NotificacionesService
├── Consumers (con InventoryConsumer)
├── Data
├── Migrations
├── Models
├── Repositories
    ├── Interfaces
    └── Impl
├── DockerFile
└── Program.cs
```

---

## ⚙ Variables de Entorno

Las conexiones a base de datos se configuran automáticamente vía `docker-compose.yml`.  
En desarrollo, podés usar `appsettings.Development.json`.

---

## 📊 Diagrama de Arquitectura

```
[ InventoryService (API REST) ]
        |       POST/PUT/DELETE
        |         
        v
[ RabbitMQ - Exchange: inventory_exchange ]
        |      RoutingKey: product.created / updated / deleted
        v
[ Queues: "product.created", "product.updated", "product.deleted" ]
        |
        v
[ NotificacionesService (BackgroundService) ]
        |
        v
[ SQL Server: NotificationDb (Tabla: InventoryLogs) ]
```

---

## 👤 Autor

Mateo Salas  
Desarrollador Backend .NET
