# IT 13 CRMS

## Setting Up (Development)
1. Create a new file in the main project folder called `appsettings.json`
2. Copy this into appsettings.json
   ```
    {
     "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyRealEstateDB;Trusted_Connection=True;"
      }
    }
   ```
3. Open Package Manager Console then type `Update-Database` to create one
4. Run the project
   
Notes: You may use different connection strings
   
