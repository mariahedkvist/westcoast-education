# Westcoast Education

This project is a prototype web app for a mock company that creates and distributes online courses in the web development field. The purpose is to showcase the development and consumption of a REST API with an MVC client app.

## Technics

This project contains a WebApi and an MVC app.

### WebApi

The WebApi is responsible for CRUD operations on three different resources:

1. Courses
2. Students
3. Teachers

These resources are stored in an SQLite database.

### MVC app

The MVC client's functionality is primarily centered around the needs of a presumed Administrator. The Administrator can manage Courses, Students and Teachers.

## Build and Run

No configuration is required to build and run this project. To call the API on a specific port, this can be adjusted in the API settings:

1. Navigate to *appsettings.Development.json* in the *root* of the API directory
2. Update the *baseURL* in the *apiSettings* object:

```json
"apiSettings":{
    "baseUrl": "http://localhost:[PORT]/api/v1"
  }
  ```

