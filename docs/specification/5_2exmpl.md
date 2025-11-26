# 5_2 Примеры

## 1. Factorial
````
func num factorial(num n)
{
    if (n <= 1) 
    {
        return 1;
    } 
    else 
    {
        return n * factorial(n - 1);
    }
};
num n;
read(n);
num result = factorial(n);
writeln(a)
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