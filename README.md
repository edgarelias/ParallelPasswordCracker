# Parallel Password Cracker
[Data parallelism](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/data-parallelism-task-parallel-library) refers to scenarios in which the same operation is performed concurrently on elements in a source collection or array.

The purpose of this program is to test the advantage of using the [.NET Task Parallel Library](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel?view=net-5.0). The program will ask the user to enter a password and a time limit to crack the password. The program will try to crack the password using a single threaded execution implementation and a parallel execution implementation. Moreover, the program uses a brute-forced algorithm - this approach is typically checking each combination of letters until the password is found.


## Requirements
- Visual Studio
- .NET desktop development package
- Namespace: System.Threading.Tasks
- Assemblies: mscorlib.dll, System.Threading.Tasks.Parallel.dll

## Example
The image below shows the scenario when the program had to cracked the password "EDGAR" in less than 4.5 seconds. As you can see, the single threaded execution test took a little bit more than 4 seconds. In the other hand, the parallel execution took less than 1 second. 
![Example 1](https://github.com/edgarelias/ParallelPasswordCracker/blob/master/assets/example1.png)

The image below shows the scenario when the program had to cracked the password "RAMON" in less than 20 seconds. As you can see, the single threaded execution test took 17.4 seconds. In the other hand, the parallel execution took less than 1 second. 
![Example 2](https://github.com/edgarelias/ParallelPasswordCracker/blob/master/assets/example2.png)
