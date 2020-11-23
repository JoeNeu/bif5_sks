## Softwarekomponentensysteme Project

This Project should simulate a post system like the DHL or UPS. Parcels can be submitted, tracked from the sender or reported at a destination from the staff. Since we use an OpenApi REST interface other systems can send or get parcels from this project. The sender can also create Webhooks to get updates from the parcel.

### .NET Core 3.0 Server

DDD Domain Driven Design is used to seperate the project into three parts. Services/BusinessLogic/DataAccess

Everything is dependency injected and every layer uses Interfaces from the next deeper layer.

The Warehouse Management Api can create or export the whole Warehouse Tree

Root Warehouse 1:n Child Warehouses 1:n Child Warehouses ... unlimited tree size ... 1:1 Truck

One root warehouse can have unlimited child warehouses. Finally the leave of this tree is a truck that has a regionGeoJson {\"type\":\"Feature\",\"geometry\":{\"type\":\"MultiPolygon\",\"coordinates\":[[[[xxx]]]]}}

The **trucks_big.json** can cover the whole map of Austria and every "real" Address as long as the Geocoding Agent (GoogleMaps or BingMaps) can find it. POST /api/warehouse

The Server converts the json into Warehouses/Transferwarehouses/Trucks depending on json values. Since all three inherit from the same class Hop.

```
if (jObject["nextHops"] != null)
	return new Warehouse();
else if (jObject["numberPlate"] != null)
	return new Truck();
else if (jObject["logisticsPartnerUrl"] != null)
	return new Transferwarehouse();
```



| NuGet Packages                                           | Version   |
| -------------------------------------------------------- | --------- |
| Automapper                                               | 9.0.0     |
| AutoMapper.Extensions.Microsoft.DependencyInjection      | 7.0.0     |
| Microsoft.EntityFrameworkCore                            | 3.0.0     |
| Microsoft.EntityFrameworkCore.SqlServer                  | 3.0.0     |
| Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite | 3.0.0     |
| Microsoft.EntityFrameworkCore.Tools                      | 3.0.0     |
| Microsoft.EntityFrameworkCore.InMemory                   | 3.0.0     |
| Microsoft.OpenApi                                        | 1.1.4     |
| Microsoft.NET.Test.Sdk                                   | 16.4.0    |
| Microsoft.AspNetCore.TestHost                            | 3.0.0     |
| Moq                                                      | 4.13.1    |
| Newtonsoft.Json                                          | 12.0.2    |
| NetTopologySuite.IO.GeoJSON                              | 2.0.1     |
| SwaggerUi                                                | 1.1.0     |
| Swashbuckle.AspNetCore                                   | 5.0.0-rc4 |
| Swashbuckle.AspNetCore.Annotations                       | 5.0.0-rc4 |
| Swashbuckle.AspNetCore.Filters                           | 5.0.0-rc4 |
| Swashbuckle.AspNetCore.SwaggerGen                        | 5.0.0-rc4 |
| Swashbuckle.AspNetCore.SwaggerUI                         | 5.0.0-rc4 |
| coverlet.msbuild                                         | 2.7.0     |
| Faker.Net                                                | 1.1.1     |
| NBuilder                                                 | 6.0.1     |
| NUnit                                                    | 3.12.0    |
| NUnit3TestAdapter                                        | 3.15.1    |
| ReportGenerator                                          | 4.3.6     |
| FluentValidation                                         | 8.5.0     |
| Geocoding.Core                                           | 4.0.1     |
| Geocoding.Google                                         | 4.0.1     |
| Geocoding.Microsoft                                      | 4.0.1     |

### Swagger OpenApi 3.0.1 contract 

```
openapi_contract.yaml
```

### Frontend

The frontend is shit since i did not have enough time to implement proper angular or node.js

### Tests

The tests are not that good too...