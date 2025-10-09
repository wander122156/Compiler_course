# Summmary
numbers = input("Enter a numbers: ").split()
numbers = [float(num) for num in numbers]
total = sum(numbers)

print("sum is: ", total)