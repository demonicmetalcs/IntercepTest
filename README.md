# Interceptest

> **Warning**
> This is a work-in-progress and not the finished product.
>
> Feel free to leave feature suggestions
>
> This relies on an experimental version of c# 12 called [Interceptors](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12#interceptors) 
> This will not be available to use until Visual Studio 17.7, preview 3 will be released

## Idea

When testing a service you could use an interface or a mock. But not all services will have a mock or your code might not use DI.
So it might me difficult to test a function or impossable to mock a function.

This project will try to use [Interceptors](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12#interceptors) to mock functions that is difficult to mock.


## Purpose

## Initial Problems

- [X] can InterceptsLocationAttribute be used with source generation
- [ ] Find method to mock
- [ ] Test/mock code and generated code in different projects
- [ ] multiple injected code for one function for different test
