// Bubble sort
#include <iostream>
#include <vector>
#include <sstream>
#include <string>

int main() {
    std::string input;
    std::cout << "Enter numbers: ";
    std::getline(std::cin, input);

    std::stringstream ss(input);
    std::vector<double> numbers;
    int num;

    while (ss >> num) {
        numbers.push_back(num);
    }

    int size = numbers.size();
    for (int i = 0; i < size - 1; i++) {
        for (int j = 0; j < size - i - 1; j++) {
            if (numbers[j] > numbers[j + 1]) {
                int temp = numbers[j];
                numbers[j] = numbers[j + 1];
                numbers[j + 1] = temp;
            }
        }
    }

    std::cout << "Sorted numbers: ";
    for (int n : numbers) {
        std::cout << n << " ";
    }
    std::cout << std::endl;

    return 0;
}