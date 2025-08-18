# Reglas de Negocio - SofkaFinance

## Dominio de Clientes (Customers)

### 1. Validaciones de Datos Personales

#### Nombre

- **Requerido**: Sí
- **Longitud mínima**: 3 caracteres
- **Formato**: Texto libre
- **Ejemplo válido**: "Carlos Alberto Ruiz Martinez"

#### Género

- **Requerido**: Sí
- **Valores válidos**: "Masculino" o "Femenino"
- **Formato**: Primera letra mayúscula, resto minúsculas
- **Ejemplo válido**: "Masculino", "Femenino"

#### Edad

- **Requerido**: Sí
- **Valor mínimo**: 18 años
- **Valor máximo**: 90 años
- **Ejemplo válido**: 25

#### Identificación

- **Requerido**: Sí
- **Formato**: Texto libre
- **Único**: Debe ser única en el sistema
- **Ejemplo válido**: "1234567890"

### 2. Datos de Contacto

#### Correo Electrónico

- **Requerido**: Sí
- **Formato**: Debe cumplir formato RFC válido de email
- **Longitud máxima**: 100 caracteres
- **Único**: Debe ser único en el sistema
- **Ejemplo válido**: "carlos.ruiz@email.com"

#### Teléfono

- **Requerido**: Sí
- **Formato**: Solo dígitos (0-9)
- **Longitud mínima**: 10 dígitos
- **Longitud máxima**: 15 dígitos
- **Ejemplo válido**: "3001234567"

#### Dirección

- **Requerido**: Sí
- **Longitud mínima**: 15 caracteres
- **Ejemplo válido**: "Calle 45 #12-34, Barrio Centro, Bogotá D.C."

### 3. Credenciales

#### Contraseña

- **Requerido**: Sí
- **Longitud mínima**: 8 caracteres
- **Ejemplo válido**: "Carlos$2025!"

### 4. Estados del Cliente

#### Estado Activo/Inactivo

- **Estado inicial**: Activo al momento de la creación
- **Operaciones permitidas**: Solo clientes activos pueden realizar operaciones bancarias
- **Validación**: Se verifica el estado antes de cualquier transacción

## Dominio de Cuentas (Accounts)

### 1. Tipos de Cuenta

#### Tipo de Cuenta

- **Requerido**: Sí
- **Valores válidos**: "Ahorro" o "Corriente"
- **Ejemplo válido**: "Ahorro"

### 2. Número de Cuenta

#### Número de Cuenta

- **Requerido**: Sí
- **Formato**: Solo dígitos (0-9)
- **Longitud mínima**: 8 dígitos
- **Longitud máxima**: 10 dígitos
- **Único**: Debe ser único en el sistema
- **Ejemplo válido**: "12345678"

### 3. Balance y Operaciones

#### Saldo Inicial

- **Valor mínimo**: 0.00
- **Formato**: Decimal con máximo 2 decimales
- **Ejemplo válido**: 1000.50

#### Estado de la Cuenta

- **Estado inicial**: Activa al momento de la creación
- **Validación**: No se pueden realizar operaciones en cuentas inactivas
- **Mensaje de error**: "No se pueden realizar operaciones en una cuenta inactiva"

#### Operaciones de Movimientos

- **Depósito**: Debe ser mayor a 0
- **Retiro**: Debe ser mayor a 0 y no exceder el saldo disponible
- **Transferencia**: Debe ser mayor a 0, cuenta origen debe tener saldo suficiente

### 4. Relación Cliente-Cuenta

#### Asociación

- **Cardinalidad**: Un cliente puede tener múltiples cuentas
- **Validación**: Cada cuenta debe estar asociada a un cliente válido y activo
- **Consultas**: Se pueden obtener todas las cuentas de un cliente específico
- **Transferencia**: Debe ser mayor a 0, cuenta origen debe tener saldo suficiente

## Dominio de Movimientos (Movements)

### 1. Tipos de Movimiento

#### Tipo de Movimiento

- **Valores válidos**: "Depósito", "Retiro", "Transferencia"
- **Ejemplo válido**: "Depósito"

### 2. Validaciones de Montos

#### Monto

- **Requerido**: Sí
- **Valor mínimo**: Mayor a 0
- **Formato**: Decimal con máximo 2 decimales
- **Ejemplo válido**: 1000.50

#### Concepto

- **Requerido**: No (opcional)
- **Longitud máxima**: 200 caracteres
- **Valor por defecto**: Se asigna automáticamente según el tipo de operación
- **Ejemplo válido**: "Pago de servicios"

### 3. Validaciones Específicas por Tipo

#### Depósito

- **Validación de monto**: Debe ser mayor a 0
- **Actualización de saldo**: Se suma al saldo actual de la cuenta
- **Estado de cuenta**: La cuenta debe estar activa

#### Retiro

- **Validación de monto**: Debe ser mayor a 0
- **Validación de saldo**: El monto no puede exceder el saldo disponible
- **Actualización de saldo**: Se resta del saldo actual de la cuenta
- **Estado de cuenta**: La cuenta debe estar activa
- **Mensaje de error**: "Saldo insuficiente para realizar el retiro"

#### Transferencia

- **Cuenta origen**: Debe existir y estar activa
- **Cuenta destino**: Debe existir y estar activa
- **Validación de monto**: Debe ser mayor a 0
- **Validación de saldo**: Cuenta origen debe tener saldo suficiente
- **Operación atómica**: Se registran dos movimientos (débito y crédito)
- **Restricción**: No se puede transferir a la misma cuenta

### 4. Auditoría y Trazabilidad

#### Registro de Movimientos

- **Fecha y hora**: Se registra automáticamente al momento de la transacción
- **Saldo anterior**: Se almacena el saldo previo a la operación
- **Saldo nuevo**: Se almacena el saldo posterior a la operación
- **ID único**: Cada movimiento tiene un identificador único (GUID)

#### Consulta de Movimientos

- **Por cuenta**: Se pueden consultar todos los movimientos de una cuenta específica
- **Por rango de fechas**: Filtrado por fecha de inicio y fecha fin
- **Ordenamiento**: Los movimientos se ordenan por fecha descendente (más recientes primero)

### 5. Reportes

#### Reporte de Movimientos por Cliente

- **Método HTTP**: POST (con cuerpo JSON)
- **Parámetros requeridos**:
  - `idCliente`: GUID del cliente
  - `fechaInicio`: Fecha de inicio del período (formato: YYYY-MM-DD)
  - `fechaFin`: Fecha de fin del período (formato: YYYY-MM-DD)
- **Funcionalidad**: Obtiene todos los movimientos de todas las cuentas del cliente en el rango de fechas especificado
- **Respuesta**: Lista ordenada por fecha descendente con detalles completos de cada movimiento

## Sistema de Autenticación

### 1. Login

#### Credenciales

- **Email**: Debe corresponder a un cliente registrado y activo
- **Contraseña**: Debe coincidir con la almacenada en el sistema
- **Validación**: Se verifica que el cliente esté activo

#### Token JWT

- **Generación**: Se emite un token JWT válido tras autenticación exitosa
- **Contenido**: Incluye ID del cliente, email, rol y datos de expiración
- **Duración**: Token válido por tiempo limitado (configurable)
- **Uso**: Requerido para operaciones que requieren autenticación

## Arquitectura del Sistema

### 1. Patrón API Gateway

#### Función del Gateway

- **Punto de entrada único**: Todas las solicitudes del cliente pasan por el gateway
- **Enrutamiento**: Dirige las solicitudes a los microservicios correspondientes
- **Puertos**: Gateway expuesto en puerto 3000, servicios internos en puertos específicos

#### Comunicación entre Servicios

- **Protocolo**: RabbitMQ para comunicación asíncrona entre microservicios
- **Patrón**: Request/Response con correlation IDs
- **Servicios**:
  - **Customers Service**: Gestión de clientes y autenticación
  - **Accounts Service**: Gestión de cuentas y movimientos

### 2. Manejo de Errores

#### Códigos de Error Estándar

- **CUSTOMER_NOT_FOUND**: Cliente no encontrado
- **ACCOUNT_NOT_FOUND**: Cuenta no encontrada
- **ACCOUNT_INACTIVE**: Cuenta inactiva
- **INSUFFICIENT_FUNDS**: Fondos insuficientes
- **AMOUNT_INVALID**: Monto inválido
- **SAME_ACCOUNT_TRANSFER**: Transferencia a la misma cuenta

#### Respuestas de Error

- **Formato consistente**: Todas las respuestas de error siguen el mismo formato JSON
- **Mensajes descriptivos**: Mensajes claros y comprensibles para el usuario
- **Códigos HTTP apropiados**: 400 para errores de validación, 404 para recursos no encontrados, etc.

## Casos de Prueba Recomendados

### Casos Exitosos

1. **Gestión de Clientes**

   - Crear cliente con todos los datos válidos
   - Autenticación exitosa con credenciales válidas
   - Consultar cliente por ID

2. **Gestión de Cuentas**

   - Crear cuenta tipo "Ahorro" para cliente activo
   - Crear cuenta tipo "Corriente" para cliente activo
   - Consultar cuentas por cliente

3. **Operaciones Financieras**
   - Realizar depósito válido en cuenta activa
   - Realizar retiro con saldo suficiente
   - Realizar transferencia entre cuentas activas con saldo suficiente
   - Consultar movimientos por cuenta
   - Generar reporte de movimientos por cliente en rango de fechas

### Casos de Error

1. **Validaciones de Cliente**

   - Cliente menor de 18 años
   - Género inválido (diferente a "Masculino"/"Femenino")
   - Contraseña menor a 8 caracteres
   - Email con formato inválido
   - Teléfono con caracteres no numéricos
   - Dirección menor a 15 caracteres
   - Email duplicado en el sistema
   - Identificación duplicada en el sistema

2. **Validaciones de Cuenta**

   - Tipo de cuenta inválido
   - Número de cuenta duplicado
   - Crear cuenta para cliente inexistente

3. **Validaciones de Movimientos**

   - Retiro mayor al saldo disponible
   - Transferencia con monto mayor al saldo disponible
   - Operaciones en cuenta inactiva
   - Transferencia a la misma cuenta
   - Monto igual o menor a cero
   - Transferencia a cuenta inexistente

4. **Autenticación y Autorización**
   - Login con email inexistente
   - Login con contraseña incorrecta
   - Operaciones sin token de autenticación
   - Token expirado

## Configuración de Pruebas

### Base URL

```
http://localhost:3000
```

### Endpoints Principales

#### Autenticación

- **POST** `/api/auth/login` - Iniciar sesión

#### Clientes

- **GET** `/api/clientes/{id}` - Obtener cliente por ID
- **GET** `/api/clientes` - Obtener todos los clientes
- **POST** `/api/clientes` - Crear nuevo cliente

#### Cuentas

- **GET** `/api/cuentas` - Obtener todas las cuentas
- **GET** `/api/cuentas/{accountId}` - Obtener cuenta por ID
- **GET** `/api/cuentas/cliente/{customerId}` - Obtener cuentas por cliente
- **POST** `/api/cuentas` - Crear nueva cuenta

#### Movimientos

- **POST** `/api/movimientos/{accountId}/deposito` - Realizar depósito
- **POST** `/api/movimientos/{accountId}/retiro` - Realizar retiro
- **POST** `/api/movimientos/transferencia` - Realizar transferencia
- **GET** `/api/movimientos/cuenta/{accountId}` - Consultar movimientos por cuenta
- **POST** `/api/movimientos/reportes` - Generar reporte de movimientos por cliente

### Datos de Prueba Sugeridos

#### Cliente de Prueba

```json
{
  "nombre": "Jose Lema",
  "genero": "Masculino",
  "edad": 35,
  "identificacion": "1234567890",
  "email": "jose.lema@banco.com",
  "telefono": "3001234567",
  "direccion": "Calle 123 #45-67, Barrio Centro, Bogotá D.C.",
  "contraseña": "JoseLema$2025"
}
```

#### Cuenta de Prueba

```json
{
  "idCliente": "{GUID-del-cliente}",
  "tipoCuenta": "Ahorro",
  "numeroCuenta": "12345678",
  "saldoInicial": 1000.0
}
```

#### Movimiento de Prueba

```json
{
  "idCuenta": "{GUID-de-la-cuenta}",
  "monto": 500.0,
  "concepto": "Depósito inicial"
}
```

### Herramientas de Prueba

#### Postman Collections

- Colección completa disponible en `/tests/api/`
- Incluye casos exitosos y casos de error
- Variables de entorno configuradas para diferentes ambientes
- Ejemplos de respuestas esperadas

#### Scripts de Prueba Automatizada

- Scripts PowerShell disponibles para pruebas rápidas
- Validación automática de respuestas
- Generación de reportes de prueba
