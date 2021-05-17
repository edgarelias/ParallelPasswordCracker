# Parallel Password Cracker
[Data parallelism](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/data-parallelism-task-parallel-library) refers to scenarios in which the same operation is performed concurrently on elements in a source collection or array.

The purpose of this program is to test the advantage of using the [.NET Task Parallel Library](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel?view=net-5.0). The program will ask the user to enter a password and a time limit to crack the password. The program will try to crack the password using a single threaded execution implementation and a parallel execution implementation.


## Requirements
- Visual Studio
- .NET desktop development package
- Namespace: System.Threading.Tasks
- Assemblies: mscorlib.dll, System.Threading.Tasks.Parallel.dll
