# 1. Create a folder first and navigate into it
mkdir MyWebApiSolution
cd MyWebApiSolution

# 2. Then create the solution inside it
dotnet new sln -n MyWebApiSolution

# 3. Create the Web API project inside this folder
dotnet new webapi -n MyWebApi

# 4. Add the project to the solution
dotnet sln add MyWebApi/MyWebApi.csproj

# 5. Run the app
cd MyWebApi
dotnet run
