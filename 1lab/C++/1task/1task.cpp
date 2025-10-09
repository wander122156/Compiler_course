// Summary
#include <iostream>
#include <vector>
#include <sstream>
#include <string>


int main() {
    std::string input;
    std::cout << "Enter numbers: ";
    getline(std::cin, input);

    std::stringstream ss(input);
    std::vector<double> numbers;
    double num;
    double total = 0.0;

    while (ss >> num) {
        numbers.push_back(num);
        total += num;
    }

    std::cout << "Summary: " << total << std::endl;
    return 0;
}