# 5_2 Примеры

## 1. Factorial
````
num n;
read(n);
num a = 1;
for(i = 1, i < n+1, i = i + 1){
    a = a * i
}
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
}
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
}
writeln(sum)
````