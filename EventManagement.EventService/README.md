# Event Management Service

A microservice for managing events in a distributed event management system.

## Features

- Create, read, update, and delete events
- Filter events by past/upcoming
- Check event capacity
- Update booking counts
- Upload event images
- RESTful API with JSON responses

## Technologies

- ASP.NET Core 9.0
- Entity Framework Core with SQLite
- AutoMapper for object mapping
- Swagger for API documentation

## API Endpoints

### Events

- `GET /api/events` - Get all events
- `GET /api/events?isPast=true|false` - Get past or upcoming events
- `GET /api/events/{id}` - Get event by ID
- `POST /api/events` - Create a new event
- `PUT /api/events/{id}` - Update an event
- `DELETE /api/events/{id}` - Delete an event

### Event Capacity

- `GET /api/events/{id}/has-capacity` - Check if event has available capacity
- `PUT /api/events/{id}/registration` - Update registration count
- `PUT /api/events/{id}/bookings` - Increment/decrement booking count

### Images

- `POST /api/events/{id}/image` - Upload an image for an event

## Setup

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 or Visual Studio Code

### Running Locally

1. Clone the repository
```bash
git clone https://github.com/yourusername/EventManagement.EventService.git
cd EventManagement.EventService
```

2. Restore dependencies
```bash
dotnet restore
```

3. Build the project
```bash
dotnet build
```

4. Run the service
```bash
dotnet run
```

The service will be available at `http://localhost:5075`.

### Docker

You can also run the service using Docker:

```bash
docker build -t event-service .
docker run -p 5075:5075 event-service
```

## API Usage Examples

### Creating an Event

```bash
curl -X POST "http://localhost:5075/api/events" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {your-token}" \
  -d '{
    "title": "Tech Conference 2025",
    "description": "Annual technology conference",
    "date": "2025-06-15T10:00:00.000Z",
    "location": "Convention Center",
    "capacity": 500
  }'
```

### Uploading an Event Image

```bash
curl -X POST "http://localhost:5075/api/events/{event-id}/image" \
  -H "Authorization: Bearer {your-token}" \
  -F "image=@/path/to/image.jpg"
```

## Configuration

The service can be configured through `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=EventDatabase.db"
  },
  "EventService": {
    "BaseUrl": "http://localhost:5075"
  }
}
```

## License

[MIT](LICENSE) 