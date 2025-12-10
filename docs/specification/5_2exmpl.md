# 5_2 Примеры

## 1. Factorial
````
func num factorial(num n) {
    if (n <= 1) {
        return 1;
    };
    return n * factorial(n - 1);
};

num fnumber;
write("Введите число (от 0 до 10): ");
readln(fnumber);

if (fnumber < 0) {
    writeln("Ошибка: факториал отрицательного числа не определен!");
} else {
    if (fnumber > 10) {
        writeln("Ошибка: число слишком большое для вычисления!");
    } else {
        num result = 1;
        num i = 1;

        while (i <= fnumber) {
            result = result * i;
            i = i + 1;
        };

        writeln("Факториал ", fnumber, " (через цикл) = ", result);
        writeln("Факториал ", fnumber, " (через рекурсию) = ", factorial(fnumber));
    };
};
````
## 2. GSD
````
num a, b, temp;
read(a,b);
while (b != 0) {
    temp = b;
    b = a % b;
    a = temp
};
writeln(a)
````
## 3. SumDigits
````
num n;
num sum = 0;
write("Введите целое число: ");
readln(n);
n = abs(n);
while(n != 0){
    sum = sum + n%10;
    n = n/10
};
writeln(sum)
````