# ElasticSearchWithNetCore

## Project Description
This project is a RESTful API developed with .NET 9 that performs basic CRUD and search operations on ElasticSearch. It demonstrates ElasticSearch's powerful search capabilities (match, fuzzy, wildcard, term, bool, exists, etc.) on Product entities with practical examples.

## Features
- Fully integrated product management with ElasticSearch
- CRUD operations (Create, Read, Update, Delete)
- Advanced search functions (match, fuzzy, wildcard, term, bool, exists)
- Minimal API architecture
- API documentation with Scalar

## Installation
1. **ElasticSearch**
   - Download and start the [ElasticSearch](https://www.elastic.co/downloads/elasticsearch) server.
   - By default, it should run at `http://localhost:9200` (as set in appsettings.json).

2. **Clone the Project**
   ```bash
   git clone <repo-url>
   cd ElasticSearchWithNetCore
   ```

3. **.NET 9 SDK**
   - Make sure you have the [Microsoft .NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) installed.

4. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

## Running the Project
To start the main API project:
```bash
cd ElasticSearchWithNetCore/ElasticSearch.API
 dotnet run
```
The API will run by default at `http://localhost:5021`.

## API Endpoints
The main endpoints are as follows:

| Method | URL | Description |
|--------|-----|-------------|
| GET    | /products | Get all products |
| GET    | /products/{id} | Get a specific product |
| POST   | /products | Add a new product |
| PUT    | /products | Update a product |
| DELETE | /products/{id} | Delete a product |
| GET    | /products-details | Detailed product search |
| GET    | /products-match/{query_keyword} | Match query |
| GET    | /products-fuzzy/{query_keyword} | Fuzzy query |
| GET    | /products-wildcard/{query_keyword} | Wildcard query |
| GET    | /products-exists | Get products with a specific field |
| GET    | /products-bool | Bool query example |
| GET    | /products-term/{query_keyword} | Term query |
| GET    | /products-count | Get total product count |

### Sample Product Model
```json
{
  "name": "Mouse",
  "quantity": 10,
  "price": 299.99
}
```

### Sample POST Request
```http
POST /products
Content-Type: application/json

{
  "name": "Keyboard",
  "quantity": 5,
  "price": 499.99
}
```

## Technologies Used
- .NET 9
- ElasticSearch 8.x
- Elastic.Clients.Elasticsearch (NuGet)
- Microsoft.AspNetCore.OpenApi
- Scalar.AspNetCore
