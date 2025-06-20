README: Aviatur Hoteles API
Este repositorio contiene la lógica de backend para el portal de reservas. Es una API RESTful desarrollada con ASP.NET Core que se encarga de la búsqueda de hoteles a través de un proveedor externo (DummyJSON, simulado) y la gestión de reservas en una base de datos SQL Server.

1. Estructura del Proyecto
La solución de la API (AviaturHoteles.sln) se organiza en varios proyectos para promover la separación de responsabilidades:

* AviaturHoteles:

  - Proyecto principal ASP.NET Core Web API.
  - Contiene la configuración del inicio de la aplicación (Program.cs), y la configuración de Swagger/OpenAPI.
  - Las configuraciones de la aplicación (como cadenas de conexión a la base de datos o URLs de proveedores externos) se gestionan en appsettings.json.

* AviaturHoteles.Controller

  - Contiene el controlador HotelContoller donde es el punto de entrada de las paticiones HTTP.

* AviaturHoteles.Models

  - Contiene las entidades usadas para el tratamiento de los datos de entrada y de salida.

* AviaturHoteles.Services:

  - Librería de clases que encapsula la lógica de negocio central del dominio de hoteles y reservas.
  - Contiene las interfaces (IHotelService, IReservaService) y sus implementaciones (HotelService, ReservaService).
  - Se encarga de la orquestación entre la API, la base de datos y los proveedores externos.

2. Prerrequisitos
Para ejecutar este proyecto, necesitas tener instalado lo siguiente:

* SDK de .NET 8.0.
* SQL Server: Una instancia de SQL Server .
* SQL Server Management Studio.
* Visual Studio 2022  o Visual Studio Code).

3. Configuración y Ejecución
Sigue estos pasos para configurar y ejecutar la API:

* Clonar el Repositorio:
    git clone <URL_DEL_REPOSITORIO_API>
    cd AviaturHoteles
    
* Configurar la Base de Datos:

  * Abre tu SSMS/Azure Data Studio.
  * Crea una nueva base de datos llamada AviaturReservasDB.
  * Ejecuta los scripts SQL para la creación de tablas, la inserción de datos iniciales y los procedimientos almacenados. Estos scripts deberían estar en una       carpeta de tu repositorio (ej. AviaturHoteles.Database o SqlScripts). Asegúrate de ejecutar:
  * Scripts de creación de tablas (Usuarios, Proveedores, Reservas, BusquedasHoteles).
  * Scripts de inserción de datos iniciales (para Proveedores y al menos un Usuario de prueba para reservas).
  * Los procedimientos almacenados: sp_InsertarBusquedaHotel, sp_InsertarReservaSabre y sp_ObtenerReporteReservas.
  * Configura la cadena de conexión: Abre el archivo appsettings.json en el proyecto AviaturHoteles.Api. Asegúrate de que la sección ConnectionStrings tenga una   entrada DefaultConnection que apunte a tu instancia de SQL Server.

* Restaurar Paquetes NuGet:

  * Abre la solución AviaturHoteles.sln en Visual Studio.
  * En el Explorador de soluciones, haz clic derecho en la solución (AviaturHoteles).
  * Selecciona "Restaurar paquetes NuGet" (o ejecuta dotnet restore desde la terminal en la carpeta de la solución).
    
* Ejecutar la API:

  - En Visual Studio, asegúrate de que el proyecto AviaturHoteles.Api esté configurado como el proyecto de inicio.
  - Presiona F5 o haz clic en el botón "Iniciar".

  - Esto iniciará la API y debería abrir automáticamente la interfaz de Swagger UI en tu navegador (ej. https://localhost:7028/swagger).

* Verificar el Puerto de la API:

  - Anote la URL base (ej. https://localhost:7028/). La necesitarás para configurar el frontend. Puedes encontrarla en las propiedades del proyecto AviaturHoteles.Api > "Depurar" > "Perfil de inicio".

4. Endpoints Clave de la API
   
  * POST /api/Hoteles/buscar:

    Descripción: Permite buscar hoteles con base en criterios como ciudad, fechas de check-in/check-out y número de huéspedes.
    Cuerpo de la Petición: BusquedaHotelRequest (JSON).
    Respuesta: Lista de HotelResponse (JSON).

  * POST /api/Hoteles/reservar:

    Descripción: Procesa la reserva de un hotel específico. Se encarga de validar el usuario (existente o crearlo si la lógica lo permite en el servicio/SP) y registrar la reserva.
    Cuerpo de la Petición: ReservaHotelRequest (JSON).
    Respuesta: ReservaHotelResponse (JSON) indicando éxito o fracaso y un mensaje.

  * GET /api/Hoteles/reporteReservas:

    Descripción: Obtiene un listado de reservas registradas, con opciones de filtrado por fechas de creación, ciudad del hotel, estado de la reserva, etc.
    Parámetros de Consulta (opcionales): fechaDesde, fechaHasta, ciudadHotel, estadoReserva, proveedorID, empresaID, usuarioID.
    Respuesta: Lista de ReservaReporteResponse (JSON).
