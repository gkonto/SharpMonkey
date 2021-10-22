## Monkey Kong In C++   <img src="conf/gorilla-facing-right.png" width="40">
Monkey is a programming language built by reading through Writing An Interpreter In Go in C#.

Monkey is implemented as a tree-walking interpreter.

Here is what Monkey looks like:

```c++
// Integers & arithmetic expressions...
let version = 1 + (50 / 2) - (8 * 3);

// ... and strings
let name = "The Monkey programming language";

// ... booleans
let isMonkeyFastNow = true;

// ... arrays & hash maps
let people = [{"name": "Anna", "age": 24}, {"name": "Bob", "age": 99}];
```

It also has functions!

```c++
// User-defined functions...
let getName = fn(person) { person["name"]; };
getName(people[0]); // => "Anna"
getName(people[1]); // => "Bob"

// and built-in functions
puts(len(people))  // prints: 2
```

And it has conditionals, implicit and explicit returns and recursive functions, which means we can do this in Monkey:

```c++
let fibonacci = fn(x) {
  if (x == 0) {
    0
  } else {
    if (x == 1) {
      return 1;
    } else {
      fibonacci(x - 1) + fibonacci(x - 2);
    }
  }
};
```

But the crown jewel in every Monkey implementation are closures:

```c++
// `newAdder` returns a closure that makes use of the free variables `a` and `b`:
let newAdder = fn(a, b) {
    fn(c) { a + b + c };
};
// This constructs a new `adder` function:
let adder = newAdder(1, 2);

adder(8); // => 11
```

## Monkey list of features

* C-like syntax
* Variable bindings
* Integers and Booleans
* Arithmetic Expressions
* Built-in functions
* First-class and higher-order functions
* Closures
* String data structure
* Array data structure[TODO]
* Hash data structure[TODO]

*Code snippets on how to use the about features can be found above and in qa folder
 

## How to install
The project contains two executables:
* the REPL.
* A Quality Assurance (qa/) mechanism for the project.

 You can compile the project using the CMAKE cross-platform software tool.
 
__REPL compilation__
dotnet run --configuration debug
dotnet run --configuration release

An executable will be created in the build/ directory

__QA mechanism compilation__
cd qa; dotnet run test



## Authors

    George Kontogiannis - Initial work

## License

This project is licensed under the MIT License - see the LICENSE.md file for details

## Acknowledgments
    Inspired by the book 'Writing an interpreter in Go" by Thorsten Ball



TODO
1) Camel case (eg const, orderOfDetail etc) Local variables, private fields.
2) Title case (eg String, Cost) Types, non-private fields, and methods.
3) Set #nullable enable and check the corresponding warnings.
4) Format filenames, _ seperated, camel case
 
let fib = fn(x) { if (x==0) { return 0; } else { if (x==1) { return 1; } else { fib(x - 1) + fib(x - 2); } } };
5) dot -Tsvg -o output.svg
6) all namespaces to "monkey"
