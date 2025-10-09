# Bubble sort
numbers = input("Enter numbers separated by space: ").split()

def bubble_sort(array):
    n = len(numbers)
    for i in range(n):
        for j in range(0, n - i - 1):
            if numbers[j] > numbers[j + 1]:
                numbers[j], numbers[j + 1] = numbers[j + 1], numbers[j]
    return array

print("sorted array: ", bubble_sort(numbers))