# Todo Task App
This application consists of both a frontend and backend that has been developed in React + Typescript + Vite and ASP.net Web API (.Net 10).  The API has been written using the Clean Architecture Design Pattern and CQRS.

## Backend
To run the web API application, first locate the `appsettings.json` file and update the database connection string to point to your local database. 
**NB: MS SQL Database was used for this project.** 

Then, run the migrations using the following command:
```
dotnet ef database update -p src/TaskApp.Infrastructure -s src/TaskApp.Api
```

## Frontend
Navigate into the `frontend > TaskAppFe` folder from your terminal/command prompt. Then run the following commands:
```
npm install
npm run dev
```
