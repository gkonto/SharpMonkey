# sharp-monkey

dotnet run --configuration debug
dotnet run --configuration release

cd qa; dotnet run test

TODO
1) Camel case (eg const, orderOfDetail etc) Local variables, private fields.
2) Title case (eg String, Cost) Types, non-private fields, and methods.
3) Set #nullable enable and check the corresponding warnings.
4) Format filenames, _ seperated, camel case
 
let fib = fn(x) { if (x==0) { return 0; } else { if (x==1) { return 1; } else { fib(x - 1) + fib(x - 2); } } };
