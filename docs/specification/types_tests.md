# FizzBuzz
```
num n;
while (true) {
    read(n);
    if (n == 0) {
        break;
    };  
    if (n % 15 == 0) {
        writeln("FizzBuzz");
    } else {
        if (n % 3 == 0) {
            writeln("Fizz");
        } else {
            if (n % 5 == 0) {
                writeln("Buzz");
            } else {
                writeln(n);
            };
        };
    };
}
```

# IsLeapYear

```
write("Введите год: ");
num year;
readln(year);

bool isLeap = false;

if (year % 400 == 0) {
    isLeap = true;
} else {
    if (year % 100 == 0) {
        isLeap = false;
    } else {
        if (year % 4 == 0) {
            isLeap = true;
        } else {
            isLeap = false;
        };
    };
};

if (isLeap) {
    writeln("yes");
} else {
    writeln("no");
};
```
