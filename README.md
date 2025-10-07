
1- Project Overview
A .NET-based Workflow Tracking System that allows organizations to define and manage workflows with validation mechanisms. The system provides REST API endpoints for workflow management, process execution, and external validation integration.

2- Repository Structure:
WorkflowTracking/
├── WorkFlowTracking.API/            # Main API project
├── WorkFlowTracking.Application/    # Application layer
├── WorkFlowTracking.Domain/         # Domain layer
├── WorkFlowTracking.Infrastructure/ # Infrastructure layer
├── ValidationAPI/                   # External Validation API
└── WorkFlowTracking.sln             # Solution file

3- Clone and Setup the Repository:
git clone https://github.com/h-korany/WorkflowTracking.git

4- Database Setup:
Using SQL Server Express/Full:
attach the database to the server and setup the database user according to connection string in appsettings.json:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WorkflowTrackingDb;User ID=workFlowUser;Password=workFlowUser@23;TrustServerCertificate=True"
  }
}

5- Running the Application
Using Visual Studio
    1- Open WorkFlowTracking.sln in Visual Studio
      then run the application
    2- Open validationAPI.sln in Visual Studio:
      then run the application  
      Check the ValidationApi:BaseUrl in main API's appsettings.json



